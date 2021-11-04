using PizzaItaliano.Services.Payments.API;
using PizzaItaliano.Services.Payments.Application.Commands;
using PizzaItaliano.Services.Payments.Application.Events;
using PizzaItaliano.Services.Payments.Application.Events.Rejected;
using PizzaItaliano.Services.Payments.Application.Exceptions;
using PizzaItaliano.Services.Payments.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Payments.Tests.Intgration.Helpers;
using PizzaItaliano.Services.Payments.Tests.Shared;
using PizzaItaliano.Services.Payments.Tests.Shared.Factories;
using PizzaItaliano.Services.Payments.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Payments.Tests.Intgration
{
    public class AddPaymentTests : IDisposable, IClassFixture<PizzaItalianoApplicationFactory<Program>>
    {
        private Task Act(AddPayment command) => _rabbitMqFixture.PublishAsync(command, Exchange);

        [Fact]
        public async Task add_payment_command_should_add_document_with_given_id_to_database()
        {
            var paymentId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var cost = decimal.One;
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<AddedPayment, PaymentDocument>(Exchange,
                    _mongoDbFixture.GetAsync, command.PaymentId);

            await Act(command);

            var document = await tcs.Task;

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.PaymentId);
            document.OrderId.ShouldBe(command.OrderId);
            document.Cost.ShouldBe(command.Cost);
        }

        [Fact]
        public async Task add_payment_command_with_invalid_order_id_should_throw_an_exception_and_send_rejected_event()
        {
            var paymentId = Guid.NewGuid();
            var orderId = Guid.Empty;
            var cost = decimal.One;
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<AddPaymentRejected>(Exchange);

            await Act(command);

            var addPaymentRejected = await tcs.Task;

            addPaymentRejected.ShouldNotBeNull();
            addPaymentRejected.ShouldBeOfType<AddPaymentRejected>();
            var exception = new InvalidOrderIdException(paymentId);
            addPaymentRejected.Code.ShouldBe(exception.Code);
            addPaymentRejected.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task add_payment_command_with_invalid_cost_should_throw_an_exception_and_send_rejected_event()
        {
            var paymentId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var cost = new decimal(-2424.21);
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<AddPaymentRejected>(Exchange);

            await Act(command);

            var addPaymentRejected = await tcs.Task;

            addPaymentRejected.ShouldNotBeNull();
            addPaymentRejected.ShouldBeOfType<AddPaymentRejected>();
            var exception = new InvalidCostException(paymentId, cost);
            addPaymentRejected.Code.ShouldBe(exception.Code);
            addPaymentRejected.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task add_payment_command_with_invalid_id_should_throw_an_exception_and_send_rejected_event()
        {
            var paymentId = Guid.NewGuid();
            await TestHelper.AddTestPayment(paymentId, _mongoDbFixture);
            var orderId = Guid.NewGuid();
            var cost = new decimal(2424);
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<AddPaymentRejected>(Exchange);

            await Act(command);

            var addPaymentRejected = await tcs.Task;

            addPaymentRejected.ShouldNotBeNull();
            addPaymentRejected.ShouldBeOfType<AddPaymentRejected>();
            var exception = new PaymentAlreadyExistsException(paymentId);
            addPaymentRejected.Code.ShouldBe(exception.Code);
            addPaymentRejected.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private const string Exchange = "payment";
        private readonly MongoDbFixture<PaymentDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;

        public AddPaymentTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _rabbitMqFixture = new RabbitMqFixture();
            _mongoDbFixture = new MongoDbFixture<PaymentDocument, Guid>("payments");
            factory.Server.AllowSynchronousIO = true;
        }

        public void Dispose()
        {
            _mongoDbFixture.Dispose();
        }

        #endregion
    }
}

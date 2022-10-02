using PizzaItaliano.Services.Payments.Application.Commands;
using PizzaItaliano.Services.Payments.Application.Events;
using PizzaItaliano.Services.Payments.Application.Events.Rejected;
using PizzaItaliano.Services.Payments.Application.Exceptions;
using PizzaItaliano.Services.Payments.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Payments.Tests.Intgration.Helpers;
using PizzaItaliano.Services.Payments.Tests.Shared;
using PizzaItaliano.Services.Payments.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Payments.Tests.Intgration.Async
{
    [Collection("Collection")]
    public class UpdatePaymentTests
    {
        private Task Act(PayForPayment command) => _rabbitMqFixture.PublishAsync(command, Exchange);

        [Fact]
        public async Task update_payment_command_should_update_document_with_given_id_to_database()
        {
            var paymentId = Guid.NewGuid();
            await TestHelper.AddTestPayment(paymentId, Guid.NewGuid(), Core.Entities.PaymentStatus.Unpaid, _mongoDbFixture);
            var command = new PayForPayment() { PaymentId = paymentId };

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<PaidPayment, PaymentDocument>(Exchange,
                    _mongoDbFixture.GetAsync, command.PaymentId);

            await Act(command);

            var document = await tcs.Task;

            document.ShouldNotBeNull();
            document.PaymentStatus.ShouldBe(Core.Entities.PaymentStatus.Paid);
        }

        [Fact]
        public async Task update_payment_command_with_invalid_id_should_throw_an_exception_and_send_rejected_event()
        {
            var paymentId = Guid.Empty;
            var command = new PayForPayment() { PaymentId = paymentId };

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<PayForPaymentRejected>(Exchange);

            await Act(command);

            var updatePaymentRejected = await tcs.Task;

            updatePaymentRejected.ShouldNotBeNull();
            updatePaymentRejected.ShouldBeOfType<PayForPaymentRejected>();
            var exception = new InvalidPaymentIdException(paymentId);
            updatePaymentRejected.Code.ShouldBe(exception.Code);
            updatePaymentRejected.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task update_payment_command_with_not_existed_payment_should_throw_an_exception_and_send_rejected_event()
        {
            var paymentId = Guid.NewGuid();
            var command = new PayForPayment() { PaymentId = paymentId };

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<PayForPaymentRejected>(Exchange);

            await Act(command);

            var updatePaymentRejected = await tcs.Task;

            updatePaymentRejected.ShouldNotBeNull();
            updatePaymentRejected.ShouldBeOfType<PayForPaymentRejected>();
            var exception = new PaymentNotFoundException(paymentId);
            updatePaymentRejected.Code.ShouldBe(exception.Code);
            updatePaymentRejected.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task update_payment_command_with_payment_status_should_throw_an_exception_and_send_rejected_event()
        {
            var paymentId = Guid.NewGuid();
            await TestHelper.AddTestPayment(paymentId, Guid.NewGuid(), Core.Entities.PaymentStatus.Paid, _mongoDbFixture);
            var command = new PayForPayment() { PaymentId = paymentId };

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<PayForPaymentRejected>(Exchange);

            await Act(command);

            var updatePaymentRejected = await tcs.Task;

            updatePaymentRejected.ShouldNotBeNull();
            updatePaymentRejected.ShouldBeOfType<PayForPaymentRejected>();
            var exception = new CannotUpdatePaymentStatusException(paymentId);
            updatePaymentRejected.Code.ShouldBe(exception.Code);
            updatePaymentRejected.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private const string Exchange = "payment";
        private readonly MongoDbFixture<PaymentDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;

        public UpdatePaymentTests(TestAppFactory factory)
        {
            _rabbitMqFixture = new RabbitMqFixture();
            _mongoDbFixture = new MongoDbFixture<PaymentDocument, Guid>("payments");
            factory.Server.AllowSynchronousIO = true;
        }

        #endregion
    }
}

using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Payments.API;
using PizzaItaliano.Services.Payments.Application.Commands;
using PizzaItaliano.Services.Payments.Application.Exceptions;
using PizzaItaliano.Services.Payments.Core.Entities;
using PizzaItaliano.Services.Payments.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Payments.Tests.EndToEnd.Helpers;
using PizzaItaliano.Services.Payments.Tests.Shared;
using PizzaItaliano.Services.Payments.Tests.Shared.Factories;
using Shouldly;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Payments.Tests.EndToEnd.Sync
{
    [Collection("Collection")]
    public class PayForPaymentTests
    {
        private Task<HttpResponseMessage> Act(PayForPayment command)
           => _httpClient.PutAsync($"payments/{command.OrderId}/pay", TestHelper.GetContent(command));

        [Fact]
        public async Task update_payment_endpoint_should_return_http_status_code_created()
        {
            var paymentId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            await TestHelper.AddTestPayment(paymentId, orderId, PaymentStatus.Unpaid, _mongoDbFixture);
            var command = new PayForPayment() { OrderId = orderId };

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task update_payment_endpoint_should_update_document_with_given_id_to_database()
        {
            var paymentId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            await TestHelper.AddTestPayment(paymentId, orderId, PaymentStatus.Unpaid, _mongoDbFixture);
            var command = new PayForPayment() { OrderId = orderId };

            await Act(command);
            var document = await _mongoDbFixture.GetAsync(paymentId);

            document.ShouldNotBeNull();
            document.PaymentStatus.ShouldBe(PaymentStatus.Paid);
        }

        [Fact]
        public async Task update_payment_endpoint_with_invalid_id_should_throw_an_exception_and_send_bad_request()
        {
            var orderId = Guid.Empty;
            var command = new PayForPayment() { OrderId = orderId };

            var response = await Act(command);
            var error = TestHelper.MapTo<TestHelper.Error>(await response.Content.ReadAsStringAsync());

            error.ShouldNotBeNull();
            error.ShouldBeOfType<TestHelper.Error>();
            var exception = new InvalidOrderIdException(orderId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task update_payment_endpoint_with_not_existed_payment_should_throw_an_exception_and_send_bad_request()
        {
            var orderId = Guid.NewGuid();
            var command = new PayForPayment() { OrderId = orderId };

            var response = await Act(command);
            var error = TestHelper.MapTo<TestHelper.Error>(await response.Content.ReadAsStringAsync());

            error.ShouldNotBeNull();
            error.ShouldBeOfType<TestHelper.Error>();
            var exception = new PaymentForOrderNotFoundException(orderId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task update_payment_endpoint_with_payment_status_paid_should_throw_an_exception_and_send_bad_request()
        {
            var paymentId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            await TestHelper.AddTestPayment(paymentId, orderId, PaymentStatus.Paid, _mongoDbFixture);
            var command = new PayForPayment() { OrderId = orderId };

            var response = await Act(command);
            var error = TestHelper.MapTo<TestHelper.Error>(await response.Content.ReadAsStringAsync());

            error.ShouldNotBeNull();
            error.ShouldBeOfType<TestHelper.Error>();
            var exception = new CannotUpdatePaymentStatusException(paymentId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<PaymentDocument, Guid> _mongoDbFixture; 

        public PayForPaymentTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<PaymentDocument, Guid>("payments");
            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
        }

        #endregion
    }
}

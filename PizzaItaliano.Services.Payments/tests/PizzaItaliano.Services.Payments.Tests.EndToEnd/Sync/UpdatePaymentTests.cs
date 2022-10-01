using PizzaItaliano.Services.Payments.API;
using PizzaItaliano.Services.Payments.Application.Commands;
using PizzaItaliano.Services.Payments.Application.Exceptions;
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
    public class UpdatePaymentTests
    {
        private Task<HttpResponseMessage> Act(PayFromPayment command)
           => _httpClient.PutAsync($"payments/{command.PaymentId}", TestHelper.GetContent(command));

        [Fact]
        public async Task update_payment_endpoint_should_return_http_status_code_created()
        {
            var paymentId = Guid.NewGuid();
            await TestHelper.AddTestPayment(paymentId, _mongoDbFixture);
            var command = new PayFromPayment() { PaymentId = paymentId };

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task update_payment_endpoint_should_return_body_with_correct_order_id()
        {
            var paymentId = Guid.NewGuid();
            await TestHelper.AddTestPayment(paymentId, _mongoDbFixture);
            var command = new PayFromPayment() { PaymentId = paymentId };
            var expectedResponse = $"payments/{paymentId}";

            var response = await Act(command);
            var bodyString = TestHelper.MapTo<string>(await response.Content.ReadAsStringAsync());

            bodyString.ShouldNotBeNull();
            bodyString.ShouldBe(expectedResponse);
        }

        [Fact]
        public async Task update_payment_endpoint_should_update_document_with_given_id_to_database()
        {
            var paymentId = Guid.NewGuid();
            await TestHelper.AddTestPayment(paymentId, Guid.NewGuid(), Core.Entities.PaymentStatus.Unpaid, _mongoDbFixture);
            var command = new PayFromPayment() { PaymentId = paymentId };

            await Act(command);
            var document = await _mongoDbFixture.GetAsync(command.PaymentId);

            document.ShouldNotBeNull();
            document.PaymentStatus.ShouldBe(Core.Entities.PaymentStatus.Paid);
        }

        [Fact]
        public async Task update_payment_endpoint_with_invalid_id_should_throw_an_exception_and_send_bad_request()
        {
            var paymentId = Guid.Empty;
            var command = new PayFromPayment() { PaymentId = paymentId };

            var response = await Act(command);
            var error = TestHelper.MapTo<TestHelper.Error>(await response.Content.ReadAsStringAsync());

            error.ShouldNotBeNull();
            error.ShouldBeOfType<TestHelper.Error>();
            var exception = new InvalidPaymentIdException(paymentId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task update_payment_endpoint_with_not_existed_payment_should_throw_an_exception_and_send_bad_request()
        {
            var paymentId = Guid.NewGuid();
            var command = new PayFromPayment() { PaymentId = paymentId };

            var response = await Act(command);
            var error = TestHelper.MapTo<TestHelper.Error>(await response.Content.ReadAsStringAsync());

            error.ShouldNotBeNull();
            error.ShouldBeOfType<TestHelper.Error>();
            var exception = new PaymentNotFoundException(paymentId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task update_payment_endpoint_with_payment_status_should_throw_an_exception_and_send_bad_request()
        {
            var paymentId = Guid.NewGuid();
            await TestHelper.AddTestPayment(paymentId, Guid.NewGuid(), Core.Entities.PaymentStatus.Paid, _mongoDbFixture);
            var command = new PayFromPayment() { PaymentId = paymentId };

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

        public UpdatePaymentTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<PaymentDocument, Guid>("payments");
            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
        }

        #endregion
    }
}

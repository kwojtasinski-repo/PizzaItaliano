using PizzaItaliano.Services.Payments.API;
using PizzaItaliano.Services.Payments.Application.Commands;
using PizzaItaliano.Services.Payments.Application.Exceptions;
using PizzaItaliano.Services.Payments.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Payments.Tests.EndToEnd.Helpers;
using PizzaItaliano.Services.Payments.Tests.Shared;
using PizzaItaliano.Services.Payments.Tests.Shared.Factories;
using PizzaItaliano.Services.Payments.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Payments.Tests.EndToEnd
{
    [Collection("Collection")]
    public class AddPaymentTests
    {
        private Task<HttpResponseMessage> Act(AddPayment command)
            => _httpClient.PostAsync("payments", TestHelper.GetContent(command));

        [Fact]
        public async Task add_product_endpoint_should_return_http_status_code_created()
        {
            var paymentId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var cost = decimal.One;
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };
            _httpClient.DefaultRequestHeaders.Add("Correlation-Context", CorrelationContextFixture.CreateSimpleContextAndReturnAsJson(UserFixture.CreateSampleUser()));

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Fact]
        public async Task add_product_endpoint_should_return_location_header_with_correct_order_id()
        {
            var paymentId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var cost = decimal.One;
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };
            _httpClient.DefaultRequestHeaders.Add("Correlation-Context", CorrelationContextFixture.CreateSimpleContextAndReturnAsJson(UserFixture.CreateSampleUser()));

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();

            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"payments/{command.PaymentId}");
        }

        [Fact]
        public async Task add_payment_endpoint_should_add_document_with_given_id_to_database()
        {
            var paymentId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var cost = decimal.One;
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };
            _httpClient.DefaultRequestHeaders.Add("Correlation-Context", CorrelationContextFixture.CreateSimpleContextAndReturnAsJson(UserFixture.CreateSampleUser()));

            await Act(command);
            var document = await _mongoDbFixture.GetAsync(command.PaymentId);

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.PaymentId);
            document.OrderId.ShouldBe(command.OrderId);
            document.Cost.ShouldBe(command.Cost);
        }

        [Fact]
        public async Task add_payment_command_with_invalid_order_id_should_return_bad_request()
        {
            var paymentId = Guid.NewGuid();
            var orderId = Guid.Empty;
            var cost = decimal.One;
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };
            _httpClient.DefaultRequestHeaders.Add("Correlation-Context", CorrelationContextFixture.CreateSimpleContextAndReturnAsJson(UserFixture.CreateSampleUser()));

            await Act(command);
            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new InvalidOrderIdException(paymentId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task add_payment_command_with_invalid_cost_should_throw_an_exception_and_send_rejected_event()
        {
            var paymentId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var cost = new decimal(-2424.21);
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };
            _httpClient.DefaultRequestHeaders.Add("Correlation-Context", CorrelationContextFixture.CreateSimpleContextAndReturnAsJson(UserFixture.CreateSampleUser()));

            await Act(command);
            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new InvalidCostException(paymentId, cost);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task add_payment_command_with_invalid_id_should_throw_an_exception_and_send_rejected_event()
        {
            var paymentId = Guid.NewGuid();
            await TestHelper.AddTestPayment(paymentId, _mongoDbFixture);
            var orderId = Guid.NewGuid();
            var cost = new decimal(2424.21);
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };
            _httpClient.DefaultRequestHeaders.Add("Correlation-Context", CorrelationContextFixture.CreateSimpleContextAndReturnAsJson(UserFixture.CreateSampleUser()));

            await Act(command);
            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new PaymentAlreadyExistsException(paymentId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }


        [Fact]
        public async Task given_header_without_user_id_when_add_payment_should_return_http_status_code_bad_request()
        {
            var paymentId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var cost = decimal.One;
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };

            await Act(command);
            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new InvalidUserIdException(Guid.Empty);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<PaymentDocument, Guid> _mongoDbFixture;

        public AddPaymentTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<PaymentDocument, Guid>("payments");
            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
        }

        #endregion
    }
}

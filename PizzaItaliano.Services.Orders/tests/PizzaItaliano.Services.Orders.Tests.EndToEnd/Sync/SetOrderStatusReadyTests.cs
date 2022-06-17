using PizzaItaliano.Services.Orders.API;
using PizzaItaliano.Services.Orders.Application.Commands;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Orders.Tests.EndToEnd.Helpers;
using PizzaItaliano.Services.Orders.Tests.Shared.Factories;
using PizzaItaliano.Services.Orders.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.EndToEnd.Sync
{
    [Collection("Collection")]
    public class SetOrderStatusReadyTests
    {
        private Task<HttpResponseMessage> Act(SetOrderStatusReady command)
            => _httpClient.PutAsync($"orders", TestHelper.GetContent(command));

        [Fact]
        public async Task set_order_status_endpoint_should_return_http_status_code_ok()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            await TestHelper.AddTestOrderWithOrderProduct(orderId, orderProductId, Core.Entities.OrderStatus.New, Core.Entities.OrderProductStatus.New, _mongoDbFixture);
            var command = new SetOrderStatusReady(orderId);
            var expectedResponse = $"Set status ready orders/{orderId}";

            var response = await Act(command);
            var bodyString = TestHelper.MapTo<string>(await response.Content.ReadAsStringAsync());

            bodyString.ShouldNotBeNull();
            bodyString.ShouldBe(expectedResponse);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task set_order_status_endpoint_should_set_document_ready_with_given_id_from_database()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            await TestHelper.AddTestOrderWithOrderProduct(orderId, orderProductId, Core.Entities.OrderStatus.New, Core.Entities.OrderProductStatus.New, _mongoDbFixture);
            var command = new SetOrderStatusReady(orderId);
            var expectedStatus = Core.Entities.OrderStatus.Ready;

            await Act(command);
            var document = await _mongoDbFixture.GetAsync(command.OrderId);

            document.ShouldNotBeNull();
            document.OrderStatus.ShouldBe(expectedStatus);
        }

        [Fact]
        public async Task set_order_status_command_with_invalid_id_should_throw_an_exception_and_send_bad_request()
        {
            var orderId = Guid.NewGuid();
            var command = new SetOrderStatusReady(orderId);

            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new OrderNotFoundException(orderId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task set_order_status_command_with_invalid_status_should_throw_an_exception_and_send_bad_request()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var command = new SetOrderStatusReady(orderId);
            await TestHelper.AddTestOrderWithOrderProduct(orderId, orderProductId, _mongoDbFixture);

            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new CannotChangeOrderStatusException(orderId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<OrderDocument, Guid> _mongoDbFixture;

        public SetOrderStatusReadyTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<OrderDocument, Guid>("orders");
            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
        }

        #endregion
    }
}

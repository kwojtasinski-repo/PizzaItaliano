using PizzaItaliano.Services.Orders.API;
using PizzaItaliano.Services.Orders.Application.Commands;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Orders.Tests.EndToEnd.Helpers;
using PizzaItaliano.Services.Orders.Tests.Shared.Factories;
using PizzaItaliano.Services.Orders.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.EndToEnd.Sync
{
    public class DeleteOrderProductTests : IDisposable, IClassFixture<PizzaItalianoApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(DeleteOrderProduct command)
            => _httpClient.DeleteAsync($"orders/{command.OrderId}/order-product/{command.OrderProductId}/quantity/{command.Quantity}");

        [Fact]
        public async Task delete_order_product_endpoint_should_return_http_status_code_ok()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            await TestHelper.AddTestOrderWithOrderProduct(orderId, orderProductId, Core.Entities.OrderStatus.New, Core.Entities.OrderProductStatus.New, _mongoDbFixture);
            int quantity = 1;
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);
            var expectedResponse = $"Deleted orders/order-product/{orderProductId} with quantity {quantity}";

            var response = await Act(command);
            var bodyString = TestHelper.MapTo<string>(await response.Content.ReadAsStringAsync());

            bodyString.ShouldNotBeNull();
            bodyString.ShouldBe(expectedResponse);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task delete_order_product_endpoint_should_delete_document_with_given_id_from_database()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            await TestHelper.AddTestOrderWithOrderProduct(orderId, orderProductId, Core.Entities.OrderStatus.New, Core.Entities.OrderProductStatus.New, _mongoDbFixture);
            int quantity = 1;
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);

            await Act(command);
            var document = await _mongoDbFixture.GetAsync(command.OrderId);

            document.ShouldNotBeNull();
            document.OrderProductDocuments.Count().ShouldBe(0);
        }

        [Fact]
        public async Task delete_order_product_command_with_invalid_quantity_should_throw_an_exception_and_send_bad_request()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            int quantity = -1;
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);

            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new CannotDeleteOrderProductException(orderId, orderProductId, quantity);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task delete_order_product_command_with_invalid_status_should_throw_an_exception_and_send_bad_request()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            await TestHelper.AddTestOrderWithOrderProduct(orderId, orderProductId, _mongoDbFixture);
            int quantity = 1;
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);

            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new CannotDeleteOrderProductException(orderId, orderProductId, quantity);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task delete_order_product_command_with_invalid_id_should_throw_an_exception_and_send_bad_request()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            int quantity = 1;
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);

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
        public async Task delete_order_product_command_without_products_should_throw_an_exception_and_send_bad_request()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            int quantity = 1;
            await TestHelper.AddTestOrder(orderId, Core.Entities.OrderStatus.New, _mongoDbFixture);
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);

            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new OrderProductNotFoundException(orderProductId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<OrderDocument, Guid> _mongoDbFixture;

        public DeleteOrderProductTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<OrderDocument, Guid>("orders");
            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
        }

        public void Dispose()
        {
            _mongoDbFixture.Dispose();
        }

        #endregion
    }
}

using PizzaItaliano.Services.Orders.API;
using PizzaItaliano.Services.Orders.Application.Commands;
using PizzaItaliano.Services.Orders.Application.DTO;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Core.Entities;
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
    public class AddOrderProductTests
    {
        private Task<HttpResponseMessage> Act(AddOrderProduct command)
            => _httpClient.PostAsync("orders/order-product", TestHelper.GetContent(command));

        [Fact]
        public async Task add_order_product_endpoint_should_return_http_status_code_created()
        {
            var orderId = Guid.NewGuid();
            await TestHelper.AddTestOrder(orderId, OrderStatus.New, _mongoDbFixture);
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            int quantity = 1;
            var cost = new decimal(100);
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);
            var product = new ProductDto() { Id = productId, Cost = cost, Name = "pr1", ProductStatus = ProductStatus.Used };
            await _redisFixture.AddObjectToCache(product, productId.ToString());

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task add_order_product_endpoint_should_return_location_header_with_correct_order_id()
        {
            var orderId = Guid.NewGuid();
            await TestHelper.AddTestOrder(orderId, OrderStatus.New, _mongoDbFixture);
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            int quantity = 1;
            var cost = new decimal(100);
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);
            var product = new ProductDto() { Id = productId, Cost = cost, Name = "pr1", ProductStatus = ProductStatus.Used };
            await _redisFixture.AddObjectToCache(product, productId.ToString());
            var expectResponse = $"orders/order-product/{orderProductId}";

            var response = await Act(command);
            var bodyString = TestHelper.MapTo<string>(await response.Content.ReadAsStringAsync());

            bodyString.ShouldNotBeNull();
            bodyString.ShouldBe(expectResponse);
        }

        [Fact]
        public async Task add_order_product_endpoint_should_add_document_with_given_id_to_database()
        {
            var orderId = Guid.NewGuid();
            await TestHelper.AddTestOrder(orderId, OrderStatus.New, _mongoDbFixture);
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            int quantity = 1;
            var cost = new decimal(100);
            var product = new ProductDto() { Id = productId, Cost = cost, Name = "pr1", ProductStatus = ProductStatus.Used };
            await _redisFixture.AddObjectToCache(product, productId.ToString());
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);
            
            await Act(command);
            var document = await _mongoDbFixture.GetAsync(command.OrderId);

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.OrderId);
        }

        [Fact]
        public async Task add_order_product_endpoint_with_invalid_quantity_should_return_http_status_code_bad_request()
        {
            var orderId = Guid.NewGuid();
            await TestHelper.AddTestOrder(orderId, OrderStatus.New, _mongoDbFixture);
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            int quantity = -1;
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);

            await Act(command);
            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new CannotAddOrderProductException(orderId, command.OrderProductId, command.ProductId, quantity);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task add_order_product_endpoint_to_invalid_order_should_return_http_status_code_bad_request()
        {
            var orderId = Guid.NewGuid();
            await TestHelper.AddTestOrder(orderId, OrderStatus.Ready, _mongoDbFixture);
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            int quantity = 1;
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);

            await Act(command);
            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new CannotAddOrderProductException(orderId, command.OrderProductId, command.ProductId, quantity);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task add_order_product_endpoint_with_invalid_id_should_return_http_status_code_bad_request()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            int quantity = 1;
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);

            await Act(command);
            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new OrderNotFoundException(orderId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<OrderDocument, Guid> _mongoDbFixture;
        private readonly RedisFixture _redisFixture;

        public AddOrderProductTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<OrderDocument, Guid>("orders");
            _httpClient = factory.CreateClient();
            factory.Server.AllowSynchronousIO = true;
            _redisFixture = new RedisFixture();
        }

        #endregion
    }
}

using Microsoft.Extensions.Caching.Distributed;
using PizzaItaliano.Services.Orders.API;
using PizzaItaliano.Services.Orders.Application.Commands;
using PizzaItaliano.Services.Orders.Application.DTO;
using PizzaItaliano.Services.Orders.Application.Events;
using PizzaItaliano.Services.Orders.Application.Events.Rejected;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Application.Services.Clients;
using PizzaItaliano.Services.Orders.Core.Entities;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Orders.Tests.Integration.Helpers;
using PizzaItaliano.Services.Orders.Tests.Shared.Factories;
using PizzaItaliano.Services.Orders.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.Integration.Async
{
    public class AddOrderProductTests : IDisposable, IClassFixture<PizzaItalianoApplicationFactory<Program>>
    {
        private Task Act(AddOrderProduct command) => _rabbitMqFixture.PublishAsync(command, Exchange);

        [Fact]
        public async Task add_order_product_command_should_add_document_with_given_id_to_database()
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

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<OrderProductAdded, OrderDocument>(Exchange,
                    _mongoDbFixture.GetAsync, command.OrderId);

            await Act(command);

            var document = await tcs.Task;

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.OrderId);
        }

        [Fact]
        public async Task add_order_product_command_with_invalid_quantity_should_throw_an_exception_and_send_rejected_event()
        {
            var orderId = Guid.NewGuid();
            await TestHelper.AddTestOrder(orderId, OrderStatus.New, _mongoDbFixture);
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            int quantity = -1;
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<AddOrderProductRejected>(Exchange);

            await Act(command);

            var addOrderRejected = await tcs.Task;

            addOrderRejected.ShouldNotBeNull();
            addOrderRejected.ShouldBeOfType<AddOrderProductRejected>();
            var exception = new CannotAddOrderProductException(orderId, command.OrderProductId, command.ProductId, quantity);
            addOrderRejected.Code.ShouldBe(exception.Code);
            addOrderRejected.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task add_order_product_command_to_invalid_order_should_throw_an_exception_and_send_rejected_event()
        {
            var orderId = Guid.NewGuid();
            await TestHelper.AddTestOrder(orderId, OrderStatus.Ready, _mongoDbFixture);
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            int quantity = 1;
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<AddOrderProductRejected>(Exchange);

            await Act(command);

            var addOrderRejected = await tcs.Task;

            addOrderRejected.ShouldNotBeNull();
            addOrderRejected.ShouldBeOfType<AddOrderProductRejected>();
            var exception = new CannotAddOrderProductException(orderId, command.OrderProductId, command.ProductId, quantity);
            addOrderRejected.Code.ShouldBe(exception.Code);
            addOrderRejected.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task add_order_product_with_invalid_id_should_throw_an_exception_and_send_rejected_event()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            int quantity = 1;
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<AddOrderProductRejected>(Exchange);

            await Act(command);

            var addOrderRejected = await tcs.Task;

            addOrderRejected.ShouldNotBeNull();
            addOrderRejected.ShouldBeOfType<AddOrderProductRejected>();
            var exception = new OrderNotFoundException(orderId);
            addOrderRejected.Code.ShouldBe(exception.Code);
            addOrderRejected.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private const string Exchange = "order";
        private readonly MongoDbFixture<OrderDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;
        private readonly RedisFixture _redisFixture;

        public AddOrderProductTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _rabbitMqFixture = new RabbitMqFixture();
            _mongoDbFixture = new MongoDbFixture<OrderDocument, Guid>("orders");
            factory.Server.AllowSynchronousIO = true;
            _redisFixture = new RedisFixture();
        }

        public void Dispose()
        {
            _mongoDbFixture.Dispose();
        }

        #endregion
    }
}

using PizzaItaliano.Services.Orders.API;
using PizzaItaliano.Services.Orders.Application.Commands;
using PizzaItaliano.Services.Orders.Application.Events;
using PizzaItaliano.Services.Orders.Application.Events.Rejected;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Orders.Tests.Integration.Helpers;
using PizzaItaliano.Services.Orders.Tests.Shared.Factories;
using PizzaItaliano.Services.Orders.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.Integration.Async
{
    public class DeleteOrderProductTests : IDisposable, IClassFixture<PizzaItalianoApplicationFactory<Program>>
    {
        private Task Act(DeleteOrderProduct command) => _rabbitMqFixture.PublishAsync(command, Exchange);

        [Fact]
        public async Task delete_order_product_command_should_delete_document_with_given_id_from_order()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            await TestHelper.AddTestOrderWithOrderProduct(orderId, orderProductId, Core.Entities.OrderStatus.New, Core.Entities.OrderProductStatus.New, _mongoDbFixture);
            int quantity = 1;
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<OrderProductDeleted, OrderDocument>(Exchange,
                    _mongoDbFixture.GetAsync, command.OrderId);

            await Act(command);

            var document = await tcs.Task;

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.OrderId);
            document.Cost.ShouldBe(decimal.Zero);
        }

        [Fact]
        public async Task delete_order_product_command_with_invalid_quantity_should_throw_an_exception_and_send_rejected_event()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            await TestHelper.AddTestOrderWithOrderProduct(orderId, orderProductId, Core.Entities.OrderStatus.New, Core.Entities.OrderProductStatus.New, _mongoDbFixture);
            int quantity = -1;
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<DeleteOrderProductRejected>(Exchange);

            await Act(command);

            var deleteOrderRejected = await tcs.Task;

            deleteOrderRejected.ShouldNotBeNull();
            deleteOrderRejected.ShouldBeOfType<DeleteOrderProductRejected>();
            var exception = new CannotDeleteOrderProductException(orderId, orderProductId, quantity);
            deleteOrderRejected.Code.ShouldBe(exception.Code);
            deleteOrderRejected.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task delete_order_product_command_with_invalid_status_should_throw_an_exception_and_send_rejected_event()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            await TestHelper.AddTestOrderWithOrderProduct(orderId, orderProductId, _mongoDbFixture);
            int quantity = 1;
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<DeleteOrderProductRejected>(Exchange);

            await Act(command);

            var deleteOrderRejected = await tcs.Task;

            deleteOrderRejected.ShouldNotBeNull();
            deleteOrderRejected.ShouldBeOfType<DeleteOrderProductRejected>();
            var exception = new CannotDeleteOrderProductException(orderId, orderProductId, quantity);
            deleteOrderRejected.Code.ShouldBe(exception.Code);
            deleteOrderRejected.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task delete_order_product_command_with_invalid_id_should_throw_an_exception_and_send_rejected_event()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            int quantity = 1;
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<DeleteOrderProductRejected>(Exchange);

            await Act(command);

            var deleteOrderRejected = await tcs.Task;

            deleteOrderRejected.ShouldNotBeNull();
            deleteOrderRejected.ShouldBeOfType<DeleteOrderProductRejected>();
            var exception = new OrderNotFoundException(orderId);
            deleteOrderRejected.Code.ShouldBe(exception.Code);
            deleteOrderRejected.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task delete_order_product_command_without_products_should_throw_an_exception_and_send_rejected_event()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            int quantity = 1;
            await TestHelper.AddTestOrder(orderId, Core.Entities.OrderStatus.New, _mongoDbFixture);
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<DeleteOrderProductRejected>(Exchange);

            await Act(command);

            var deleteOrderRejected = await tcs.Task;

            deleteOrderRejected.ShouldNotBeNull();
            deleteOrderRejected.ShouldBeOfType<DeleteOrderProductRejected>();
            var exception = new OrderProductNotFoundException(orderProductId);
            deleteOrderRejected.Code.ShouldBe(exception.Code);
            deleteOrderRejected.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private const string Exchange = "order";
        private readonly MongoDbFixture<OrderDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;

        public DeleteOrderProductTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _rabbitMqFixture = new RabbitMqFixture();
            _mongoDbFixture = new MongoDbFixture<OrderDocument, Guid>("orders");
            var testServer = factory.GetTestServer();
            testServer.AllowSynchronousIO = true;
        }

        public void Dispose()
        {
            _mongoDbFixture.Dispose();
        }

        #endregion
    }
}

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
    public class SetOrderStatusReadyTests : IDisposable, IClassFixture<PizzaItalianoApplicationFactory<Program>>
    {
        private Task Act(SetOrderStatusReady command) => _rabbitMqFixture.PublishAsync(command, Exchange);

        [Fact]
        public async Task set_order_status_command_should_set_status_with_given_id()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            await TestHelper.AddTestOrderWithOrderProduct(orderId, orderProductId, Core.Entities.OrderStatus.New, Core.Entities.OrderProductStatus.New, _mongoDbFixture);
            var command = new SetOrderStatusReady(orderId);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<OrderStateModified, OrderDocument>(Exchange,
                    _mongoDbFixture.GetAsync, command.OrderId);

            await Act(command);

            var document = await tcs.Task;

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.OrderId);
            document.OrderStatus.ShouldBe(Core.Entities.OrderStatus.Ready);
        }

        [Fact]
        public async Task set_order_status_command_with_invalid_id_should_throw_an_exception_and_send_rejected_event()
        {
            var orderId = Guid.NewGuid();
            var command = new SetOrderStatusReady(orderId);
            
            var tcs = _rabbitMqFixture
                .SubscribeAndGet<UpdateOrderRejected>(Exchange);

            await Act(command);

            var updateOrderRejected = await tcs.Task;

            updateOrderRejected.ShouldNotBeNull();
            updateOrderRejected.ShouldBeOfType<UpdateOrderRejected>();
            var exception = new OrderNotFoundException(orderId);
            updateOrderRejected.Code.ShouldBe(exception.Code);
            updateOrderRejected.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task set_order_status_command_with_invalid_status_should_throw_an_exception_and_send_rejected_event()
        {
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var command = new SetOrderStatusReady(orderId);
            await TestHelper.AddTestOrderWithOrderProduct(orderId, orderProductId, _mongoDbFixture);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<UpdateOrderRejected>(Exchange);

            await Act(command);

            var updateOrderRejected = await tcs.Task;

            updateOrderRejected.ShouldNotBeNull();
            updateOrderRejected.ShouldBeOfType<UpdateOrderRejected>();
            var exception = new CannotChangeOrderStatusException(orderId);
            updateOrderRejected.Code.ShouldBe(exception.Code);
            updateOrderRejected.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private const string Exchange = "order";
        private readonly MongoDbFixture<OrderDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;

        public SetOrderStatusReadyTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _rabbitMqFixture = new RabbitMqFixture();
            _mongoDbFixture = new MongoDbFixture<OrderDocument, Guid>("orders");
            factory.Server.AllowSynchronousIO = true;
        }

        public void Dispose()
        {
            _mongoDbFixture.Dispose();
        }

        #endregion
    }
}

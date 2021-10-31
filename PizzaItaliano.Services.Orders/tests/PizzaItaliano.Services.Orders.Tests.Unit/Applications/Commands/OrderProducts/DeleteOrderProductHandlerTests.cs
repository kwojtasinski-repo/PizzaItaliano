using Convey.CQRS.Events;
using NSubstitute;
using PizzaItaliano.Services.Orders.Application.Commands;
using PizzaItaliano.Services.Orders.Application.Commands.Handlers;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Application.Services;
using PizzaItaliano.Services.Orders.Core.Entities;
using PizzaItaliano.Services.Orders.Core.Repositories;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.Unit.Applications.Commands.OrderProducts
{
    public class DeleteOrderProductHandlerTests
    {
        private Task Act(DeleteOrderProduct command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_valid_parameters_order_product_should_be_deleted()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var quantity = 1;
            var cost = new decimal(10);
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);
            var orderProduct = new OrderProduct(orderProductId, quantity, cost, orderId, productId, OrderProductStatus.New);
            var order = new Order(orderId, "abc", cost, OrderStatus.New, DateTime.Now, null, new List<OrderProduct> { orderProduct });
            _orderRepository.GetAsync(orderId).Returns(order);

            // Act
            await Act(command);

            // Assert
            await _orderRepository.Received(1).UpdateAsync(Arg.Any<Order>());
            await _messageBroker.Received(1).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_invalid_quantity_should_throw_an_exception()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var quantity = -1;
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotDeleteOrderProductException>();
        }

        [Fact]
        public async Task given_invalid_order_id_should_throw_an_exception()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var quantity = 1;
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<OrderNotFoundException>();
        }

        [Fact]
        public async Task given_order_with_invalid_status_should_throw_an_exception()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var quantity = 1;
            var command = new DeleteOrderProduct(orderId, orderProductId, quantity);
            var order = new Order(orderId, "abc", decimal.Zero, OrderStatus.Ready, DateTime.Now, null);
            _orderRepository.GetAsync(orderId).Returns(order);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotDeleteOrderProductException>();
        }

        #region Arrange

        private readonly DeleteOrderProductHandler _handler;
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public DeleteOrderProductHandlerTests()
        {
            _orderRepository = Substitute.For<IOrderRepository>();
            _messageBroker = Substitute.For<IMessageBroker>();
            _eventMapper = Substitute.For<IEventMapper>();
            _handler = new DeleteOrderProductHandler(_orderRepository, _messageBroker, _eventMapper);
        }

        #endregion
    }
}

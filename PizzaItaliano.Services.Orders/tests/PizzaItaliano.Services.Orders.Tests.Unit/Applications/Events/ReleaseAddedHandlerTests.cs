using Convey.CQRS.Events;
using NSubstitute;
using PizzaItaliano.Services.Orders.Application.Events.External;
using PizzaItaliano.Services.Orders.Application.Events.External.Handlers;
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

namespace PizzaItaliano.Services.Orders.Tests.Unit.Applications.Events
{
    public class ReleaseAddedHandlerTests
    {
        private Task Act(ReleaseAdded command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_valid_parameters_order_should_mark_as_released()
        {
            // Arrange
            var releaseId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new ReleaseAdded(releaseId, orderId, orderProductId);
            var quantity = 1;
            var cost = new decimal(10);
            var orderProduct = new OrderProduct(orderProductId, quantity, cost, orderId, productId, OrderProductStatus.Paid);
            var order = new Order(orderId, "abc", decimal.Zero, OrderStatus.Paid, DateTime.Now, null, new List<OrderProduct> { orderProduct });
            _orderRepository.GetAsync(orderId).Returns(order);

            // Act
            await Act(command);

            // Assert
            await _orderRepository.Received(1).UpdateAsync(Arg.Any<Order>());
            await _messageBroker.Received(2).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_invalid_order_id_shouldnt_mark_as_released()
        {
            // Arrange
            var releaseId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var command = new ReleaseAdded(releaseId, orderId, orderProductId);

            // Act
            await Act(command);

            // Assert
            await _orderRepository.Received(0).UpdateAsync(Arg.Any<Order>());
            await _messageBroker.Received(0).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_order_with_invalid_status_shouldnt_mark_as_released()
        {
            // Arrange
            var releaseId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var command = new ReleaseAdded(releaseId, orderId, orderProductId);
            var order = new Order(orderId, "abc", decimal.Zero, OrderStatus.New, DateTime.Now, null);
            _orderRepository.GetAsync(orderId).Returns(order);

            // Act
            await Act(command);

            // Assert
            await _orderRepository.Received(0).UpdateAsync(Arg.Any<Order>());
            await _messageBroker.Received(0).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_invalid_order_should_throw_an_exception()
        {
            // Arrange
            var releaseId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var command = new ReleaseAdded(releaseId, orderId, orderProductId);
            var order = new Order(orderId, "abc", decimal.Zero, OrderStatus.Paid, DateTime.Now, null);
            _orderRepository.GetAsync(orderId).Returns(order);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotChangeOrderStatusException>();
        }

        #region Arrange

        private readonly ReleaseAddedHandler _handler;
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public ReleaseAddedHandlerTests()
        {
            _orderRepository = Substitute.For<IOrderRepository>();
            _messageBroker = Substitute.For<IMessageBroker>();
            _eventMapper = Substitute.For<IEventMapper>();
            _handler = new ReleaseAddedHandler(_orderRepository, _messageBroker, _eventMapper);
        }

        #endregion
    }
}

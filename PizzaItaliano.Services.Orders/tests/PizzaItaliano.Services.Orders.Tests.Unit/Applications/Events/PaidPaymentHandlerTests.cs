using Convey.CQRS.Events;
using NSubstitute;
using PizzaItaliano.Services.Orders.Application.Events.External;
using PizzaItaliano.Services.Orders.Application.Events.External.Handlers;
using PizzaItaliano.Services.Orders.Application.Services;
using PizzaItaliano.Services.Orders.Core.Entities;
using PizzaItaliano.Services.Orders.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.Unit.Applications.Events
{
    public class PaidPaymentHandlerTests
    {
        private Task Act(PaidPayment command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_valid_parameters_order_should_mark_as_paid()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            var command = new PaidPayment(paymentId, orderId);
            var order = new Order(orderId, "abc", decimal.Zero, OrderStatus.Ready, DateTime.Now, null);
            _orderRepository.GetAsync(orderId).Returns(order);

            // Act
            await Act(command);

            // Assert
            await _orderRepository.Received(1).UpdateAsync(Arg.Any<Order>());
            await _messageBroker.Received(1).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_order_with_invalid_status_shouldnt_mark_as_paid()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            var command = new PaidPayment(paymentId, orderId);
            var order = new Order(orderId, "abc", decimal.Zero, OrderStatus.New, DateTime.Now, null);
            _orderRepository.GetAsync(orderId).Returns(order);

            // Act
            await Act(command);

            // Assert
            await _orderRepository.Received(0).UpdateAsync(Arg.Any<Order>());
            await _messageBroker.Received(0).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_invalid_order_id_shouldnt_mark_as_paid()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            var command = new PaidPayment(paymentId, orderId);

            // Act
            await Act(command);

            // Assert
            await _orderRepository.Received(0).UpdateAsync(Arg.Any<Order>());
            await _messageBroker.Received(0).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        #region Arrange

        private readonly PaidPaymentHandler _handler;
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public PaidPaymentHandlerTests()
        {
            _orderRepository = Substitute.For<IOrderRepository>();
            _messageBroker = Substitute.For<IMessageBroker>();
            _eventMapper = Substitute.For<IEventMapper>();
            _handler = new PaidPaymentHandler(_orderRepository, _messageBroker, _eventMapper);
        }

        #endregion
    }
}

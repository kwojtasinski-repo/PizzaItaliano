using Castle.Core.Logging;
using Convey.CQRS.Commands;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PizzaItaliano.Services.Payments.Application.Commands;
using PizzaItaliano.Services.Payments.Application.DTO;
using PizzaItaliano.Services.Payments.Application.Events.External;
using PizzaItaliano.Services.Payments.Application.Events.External.Handlers;
using PizzaItaliano.Services.Payments.Application.Services.Clients;
using PizzaItaliano.Services.Payments.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Payments.Tests.Unit.Application.Events
{
    public class OrderStateModifiedHandlerTests
    {
        private Task Act(OrderStateModified command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_valid_parameters_payment_should_be_added()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var statusBefore = OrderStatus.New;
            var statusAfter = OrderStatus.Ready;
            var cost = decimal.One;
            var command = new OrderStateModified(orderId, statusBefore, statusAfter);
            var order = new OrderDto() { Id = orderId, Cost = cost, OrderDate = DateTime.Now, OrderNumber = "123", OrderStatus = OrderStatus.Paid };
            _orderServiceClient.GetAsync(orderId).Returns(order);

            // Act
            await Act(command);

            // Assert
            await _addPaymentCommandHandler.Received(1).HandleAsync(Arg.Any<AddPayment>());
        }

        [Fact]
        public async Task given_invalid_order_state_before_shouldnt_add_payment()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var statusBefore = OrderStatus.Ready;
            var statusAfter = OrderStatus.Ready;
            var cost = decimal.One;
            var command = new OrderStateModified(orderId, statusBefore, statusAfter);
            var order = new OrderDto() { Id = orderId, Cost = cost, OrderDate = DateTime.Now, OrderNumber = "123", OrderStatus = OrderStatus.Paid };
            _orderServiceClient.GetAsync(orderId).Returns(order);

            // Act
            await Act(command);

            // Assert
            await _addPaymentCommandHandler.Received(0).HandleAsync(Arg.Any<AddPayment>());
        }

        [Fact]
        public async Task given_invalid_order_state_after_shouldnt_add_payment()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var statusBefore = OrderStatus.New;
            var statusAfter = OrderStatus.Released;
            var cost = decimal.One;
            var command = new OrderStateModified(orderId, statusBefore, statusAfter);
            var order = new OrderDto() { Id = orderId, Cost = cost, OrderDate = DateTime.Now, OrderNumber = "123", OrderStatus = OrderStatus.Paid };
            _orderServiceClient.GetAsync(orderId).Returns(order);

            // Act
            await Act(command);

            // Assert
            await _addPaymentCommandHandler.Received(0).HandleAsync(Arg.Any<AddPayment>());
        }

        [Fact]
        public async Task given_not_existed_order_shouldnt_add_payment()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var statusBefore = OrderStatus.New;
            var statusAfter = OrderStatus.Ready;
            var command = new OrderStateModified(orderId, statusBefore, statusAfter);

            // Act
            await Act(command);

            // Assert
            await _addPaymentCommandHandler.Received(0).HandleAsync(Arg.Any<AddPayment>());
        }

        #region Arrange

        private readonly OrderStateModifiedHandler _handler;
        private readonly IOrderServiceClient _orderServiceClient;
        private readonly ICommandHandler<AddPayment> _addPaymentCommandHandler;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<OrderStateModifiedHandler> _logger;

        public OrderStateModifiedHandlerTests()
        {
            _orderServiceClient = Substitute.For<IOrderServiceClient>();
            _addPaymentCommandHandler = Substitute.For<ICommandHandler<AddPayment>>();
            _paymentRepository = Substitute.For<IPaymentRepository>();
            _logger = Substitute.For<ILogger<OrderStateModifiedHandler>>();
            _handler = new OrderStateModifiedHandler(_orderServiceClient, _addPaymentCommandHandler, _paymentRepository,
                _logger);
        }

        #endregion
    }
}

using Convey.CQRS.Events;
using Convey.MessageBrokers;
using NSubstitute;
using PizzaItaliano.Services.Orders.Application;
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

namespace PizzaItaliano.Services.Orders.Tests.Unit.Applications.Commands.Orders
{
    public class SetOrderStatusReadyHandlerTests
    {
        private Task Act(SetOrderStatusReady command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_valid_parameters_order_should_be_set_as_ready()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new SetOrderStatusReady(id);
            var order = new Order(id, "abc", decimal.Zero, OrderStatus.New, DateTime.Now, null, Guid.NewGuid());
            _orderRepository.GetAsync(id).Returns(order);

            // Act
            await Act(command);

            // Assert
            await _orderRepository.UpdateAsync(Arg.Any<Order>());
            await _messageBroker.PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_invalid_id_should_throw_an_exception()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new SetOrderStatusReady(id);
            _orderRepository.ExistsAsync(id).Returns(true);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<OrderNotFoundException>();
        }

        [Fact]
        public async Task given_invalid_order_should_throw_an_exception()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new SetOrderStatusReady(id);
            var order = new Order(id, "abc", decimal.Zero, OrderStatus.Released, DateTime.Now, null, Guid.NewGuid());
            _orderRepository.GetAsync(id).Returns(order);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotChangeOrderStatusException>();
        }

        #region Arrange

        private readonly SetOrderStatusReadyHandler _handler;
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;
        private readonly IAppContext _appContext;

        public SetOrderStatusReadyHandlerTests()
        {
            _orderRepository = Substitute.For<IOrderRepository>();
            _messageBroker = Substitute.For<IMessageBroker>();
            _eventMapper = Substitute.For<IEventMapper>();
            _appContext = Substitute.For<IAppContext>();
            _correlationContextAccessor = Substitute.For<ICorrelationContextAccessor>();
            _handler = new SetOrderStatusReadyHandler(_orderRepository, _messageBroker, _eventMapper,
                _correlationContextAccessor, _appContext);
        }

        #endregion
    }
}

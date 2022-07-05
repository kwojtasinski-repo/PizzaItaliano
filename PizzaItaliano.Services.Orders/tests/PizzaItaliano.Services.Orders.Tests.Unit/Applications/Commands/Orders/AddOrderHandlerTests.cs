using Convey.CQRS.Events;
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
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.Unit.Applications.Commands.Orders
{
    public class AddOrderHandlerTests
    {
        private Task Act(AddOrder command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_valid_parameters_order_should_be_created()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new AddOrder(id);

            // Act
            await Act(command);

            // Assert
            await _orderRepository.Received(1).AddAsync(Arg.Any<Order>());
            await _messageBroker.Received(1).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_valid_parameters_order_should_be_created_with_next_order_number()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new AddOrder(id);
            var number = 1021;
            var orderNumber = $"ORD/2021/10/31/{number}";
            var order = new Order(Guid.NewGuid(), orderNumber, new decimal(100), OrderStatus.Released, DateTime.Now, DateTime.Now, Guid.NewGuid());
            var orders = new List<Order>{ order };
            var queryableOrders = Queryable.AsQueryable(orders);
            _orderRepository.GetCollection(Arg.Any<Expression<Func<Order, bool>>>()).Returns(queryableOrders);
            
            // Act
            await Act(command);

            // Assert
            await _orderRepository.Received(1).AddAsync(Arg.Any<Order>());
            await _messageBroker.Received(1).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_existed_order_should_throw_an_exception()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new AddOrder(id);
            _orderRepository.ExistsAsync(id).Returns(true);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<OrderAlreadyExistsException>();
        }

        #region Arrange

        private readonly AddOrderHandler _handler;
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;
        private readonly IAppContext _appContext;
        private readonly IIdentityContext _identityContext;

        public AddOrderHandlerTests()
        {
            _orderRepository = Substitute.For<IOrderRepository>();
            _messageBroker = Substitute.For<IMessageBroker>();
            _eventMapper = Substitute.For<IEventMapper>();
            _appContext = Substitute.For<IAppContext>();
            _appContext.RequestId.Returns(Guid.NewGuid().ToString("N"));
            _identityContext = Substitute.For<IIdentityContext>();
            _identityContext.Id.Returns(Guid.NewGuid());
            _identityContext.Role.Returns("admin");
            _identityContext.IsAuthenticated.Returns(true);
            _identityContext.IsAdmin.Returns(true);
            _identityContext.Claims.Returns(new Dictionary<string, string>());
            _appContext.Identity.Returns(_identityContext);
            _handler = new AddOrderHandler(_orderRepository, _messageBroker, _eventMapper, _appContext);
        }

        #endregion
    }
}

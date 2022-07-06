using Convey.CQRS.Events;
using NSubstitute;
using PizzaItaliano.Services.Payments.Application;
using PizzaItaliano.Services.Payments.Application.Commands;
using PizzaItaliano.Services.Payments.Application.Commands.Handlers;
using PizzaItaliano.Services.Payments.Application.Exceptions;
using PizzaItaliano.Services.Payments.Application.Services;
using PizzaItaliano.Services.Payments.Core.Entities;
using PizzaItaliano.Services.Payments.Core.Repositories;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Payments.Tests.Unit.Application.Commands
{
    public class AddPaymentHandlerTests
    {
        private Task Act(AddPayment command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_valid_parameters_payment_should_be_added()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var cost = new decimal(100);
            var orderId = Guid.NewGuid();
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };

            // Act
            await Act(command);

            // Assert
            await _paymentRepository.Received(1).AddAsync(Arg.Any<Payment>());
            await _messageBroker.Received(1).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_invalid_cost_should_throw_an_exception()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var cost = new decimal(-100);
            var orderId = Guid.NewGuid();
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidCostException>();
        }

        [Fact]
        public async Task given_invalid_order_id_should_throw_an_exception()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var cost = new decimal(100);
            var orderId = Guid.Empty;
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidOrderIdException>();
        }

        [Fact]
        public async Task given_existed_payment_should_throw_an_exception()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var cost = new decimal(100);
            var orderId = Guid.NewGuid();
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };
            _paymentRepository.ExistsAsync(paymentId).Returns(true);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<PaymentAlreadyExistsException>();
        }

        [Fact]
        public async Task given_valid_parameters_payment_should_be_created_with_next_order_number()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var cost = new decimal(100);
            var orderId = Guid.NewGuid();
            var number = 1021;
            var paymentNumber = $"PAY/2021/10/31/{number}";
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };
            var userId = Guid.NewGuid();
            var payment = new Payment(paymentId, paymentNumber, new decimal(100), orderId, DateTime.Now, DateTime.Now, PaymentStatus.Unpaid, userId);
            var orders = new List<Payment> { payment };
            var queryablePayments = Queryable.AsQueryable(orders);
            _paymentRepository.GetCollection(Arg.Any<Expression<Func<Payment, bool>>>()).Returns(queryablePayments);

            // Act
            await Act(command);

            // Assert
            await _paymentRepository.Received(1).AddAsync(Arg.Any<Payment>());
            await _messageBroker.Received(1).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_empty_user_id_should_throw_an_exception()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var cost = new decimal(100);
            var orderId = Guid.NewGuid();
            var command = new AddPayment() { PaymentId = paymentId, Cost = cost, OrderId = orderId };
            _identityContext.Id.Returns(Guid.Empty);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidUserIdException>();
        }

        #region Arrange

        private readonly AddPaymentHandler _handler;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;
        private readonly IAppContext _appContext;
        private readonly IIdentityContext _identityContext;

        public AddPaymentHandlerTests()
        {
            _paymentRepository = Substitute.For<IPaymentRepository>();
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
            _handler = new AddPaymentHandler(_paymentRepository, _messageBroker, _eventMapper, _appContext);
        }

        #endregion
    }
}

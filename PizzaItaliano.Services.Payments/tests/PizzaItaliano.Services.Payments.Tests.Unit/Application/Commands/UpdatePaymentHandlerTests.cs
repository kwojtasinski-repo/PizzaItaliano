using Convey.CQRS.Events;
using NSubstitute;
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
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Payments.Tests.Unit.Application.Commands
{
    public class UpdatePaymentHandlerTests
    {
        private Task Act(UpdatePayment command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_valid_parameters_payment_should_be_updated()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var command = new UpdatePayment() { PaymentId = paymentId };
            var paymentNumber = "PAY/2021/10/31/1";
            var cost = decimal.One;
            var status = PaymentStatus.Unpaid;
            var payment = new Payment(paymentId, paymentNumber, cost, orderId, DateTime.Now, DateTime.Now, status);
            _paymentRepository.GetAsync(paymentId).Returns(payment);

            // Act
            await Act(command);

            // Assert
            await _paymentRepository.Received(1).UpdateAsync(Arg.Any<Payment>());
            await _messageBroker.Received(1).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_invalid_payment_id_should_throw_an_exception()
        {
            // Arrange
            var paymentId = Guid.Empty;
            var command = new UpdatePayment() { PaymentId = paymentId };

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidPaymentIdException>();
        }

        [Fact]
        public async Task given_not_existed_payment_should_throw_an_exception()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var command = new UpdatePayment() { PaymentId = paymentId };

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<PaymentNotFoundException>();
        }

        [Fact]
        public async Task given_invalid_payment_should_throw_an_exception()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var command = new UpdatePayment() { PaymentId = paymentId };
            var orderId = Guid.NewGuid();
            var paymentNumber = "PAY/2021/10/31/1";
            var cost = decimal.One;
            var status = PaymentStatus.Paid;
            var payment = new Payment(paymentId, paymentNumber, cost, orderId, DateTime.Now, DateTime.Now, status);
            _paymentRepository.GetAsync(paymentId).Returns(payment);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotUpdatePaymentStatusException>();
        }

        #region Arrange

        private readonly UpdatePaymentHandler _handler;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public UpdatePaymentHandlerTests()
        {
            _paymentRepository = Substitute.For<IPaymentRepository>();
            _messageBroker = Substitute.For<IMessageBroker>();
            _eventMapper = Substitute.For<IEventMapper>();
            _handler = new UpdatePaymentHandler(_paymentRepository, _messageBroker, _eventMapper);
        }

        #endregion
    }
}

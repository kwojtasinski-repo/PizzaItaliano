using PizzaItaliano.Services.Payments.Core.Entities;
using PizzaItaliano.Services.Payments.Core.Events;
using PizzaItaliano.Services.Payments.Core.Exceptions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Payments.Tests.Unit.Core.Entities
{
    public class CreatePaymentTests
    {
        private Payment Act(AggregateId id, string number, decimal cost, Guid orderId, PaymentStatus paymentStatus, Guid userId) => Payment.Create(id, number, cost, orderId, paymentStatus, userId);

        [Fact]
        public void given_valid_parameters_payment_should_be_created()
        {
            // Arrange
            var id = new AggregateId();
            var number = "123";
            var cost = decimal.One;
            var orderId = Guid.NewGuid();
            var status = PaymentStatus.Paid;
            var userId = Guid.NewGuid();

            // Act
            var payment = Act(id, number, cost, orderId, status, userId);

            // Assert
            payment.Id.ShouldBe(id);
            payment.PaymentNumber.ShouldBe(number);
            payment.Cost.ShouldBe(cost);
            payment.OrderId.ShouldBe(orderId);
            payment.PaymentStatus.ShouldBe(status);
            var @event = payment.Events.Single();
            @event.ShouldBeOfType<CreatePayment>();
        }

        [Fact]
        public void given_invalid_cost_should_throw_an_exception()
        {
            // Arrange
            var id = new AggregateId();
            var number = "123";
            var cost = new decimal(-20);
            var orderId = Guid.NewGuid();
            var status = PaymentStatus.Paid;
            var userId = Guid.NewGuid();

            // Act
            var exception = Record.Exception(() => Act(id, number, cost, orderId, status, userId));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidPaymentCostException>();
        }

        [Fact]
        public void given_invalid_payment_number_should_throw_an_exception()
        {
            // Arrange
            var id = new AggregateId();
            var number = "";
            var cost = decimal.One;
            var orderId = Guid.NewGuid();
            var status = PaymentStatus.Paid;
            var userId = Guid.NewGuid();

            // Act
            var exception = Record.Exception(() => Act(id, number, cost, orderId, status, userId));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidPaymentNumberException>();
        }
    }
}

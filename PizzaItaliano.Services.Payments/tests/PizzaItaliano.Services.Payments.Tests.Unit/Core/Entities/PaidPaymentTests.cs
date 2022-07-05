using PizzaItaliano.Services.Payments.Core.Entities;
using PizzaItaliano.Services.Payments.Core.Events;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Payments.Tests.Unit.Core.Entities
{
    public class PaidPaymentTests
    {
        [Fact]
        public void given_valid_parameters_payment_should_be_mark_as_paid()
        {
            // Arrange
            var id = new AggregateId();
            var number = "123";
            var cost = decimal.One;
            var orderId = Guid.NewGuid();
            var status = PaymentStatus.Unpaid;
            var statusExpected = PaymentStatus.Paid;
            var userId = Guid.NewGuid();
            var payment = new Payment(id, number, cost, orderId, DateTime.Now, DateTime.Now, status, userId);

            // Act
            payment.MarkAsPaid();

            // Assert
            payment.PaymentStatus.ShouldBe(statusExpected);
            var @event = payment.Events.Single();
            @event.ShouldBeOfType<PaymentPaid>();
        }

        [Fact]
        public void given_invalid_payment_shouldnt_mark_as_paid()
        {
            // Arrange
            var id = new AggregateId();
            var number = "123";
            var cost = decimal.One;
            var orderId = Guid.NewGuid();
            var status = PaymentStatus.Paid;
            var statusExpected = PaymentStatus.Paid;
            var userId = Guid.NewGuid();
            var payment = new Payment(id, number, cost, orderId, DateTime.Now, DateTime.Now, status, userId);

            // Act
            payment.MarkAsPaid();

            // Assert
            payment.PaymentStatus.ShouldBe(statusExpected);
        }
    }
}

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
    public class CreatePaymentTests
    {
        private Payment Act(AggregateId id, string number, decimal cost, Guid orderId, PaymentStatus paymentStatus) => Payment.Create(id, number, cost, orderId, paymentStatus);

        [Fact]
        public void given_valid_parameters_payment_should_be_created()
        {
            // Arrange
            var id = new AggregateId();
            var number = "123";
            var cost = decimal.One;
            var orderId = Guid.NewGuid();
            var status = PaymentStatus.Paid;

            // Act
            var payment = Act(id, number, cost, orderId, status);

            // Assert
            payment.Id.ShouldBe(id);
            payment.PaymentNumber.ShouldBe(number);
            payment.Cost.ShouldBe(cost);
            payment.OrderId.ShouldBe(orderId);
            payment.PaymentStatus.ShouldBe(status);
            var @event = payment.Events.Single();
            @event.ShouldBeOfType<CreatePayment>();
        }
    }
}

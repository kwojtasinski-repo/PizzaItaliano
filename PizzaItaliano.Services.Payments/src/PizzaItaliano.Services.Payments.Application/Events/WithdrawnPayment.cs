using Convey.CQRS.Events;
using System;

namespace PizzaItaliano.Services.Payments.Application.Events
{
    public class WithdrawnPayment : IEvent
    {
        public Guid PaymentId { get; }
        public Guid OrderId { get; }

        public WithdrawnPayment(Guid paymentId, Guid orderId)
        {
            PaymentId = paymentId;
            OrderId = orderId;
        }
    }
}

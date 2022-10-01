using Convey.CQRS.Events;
using Convey.MessageBrokers;
using System;

namespace PizzaItaliano.Services.Orders.Application.Events.External
{
    [Message("payment")] // binding do odpowiedniej wymiany
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

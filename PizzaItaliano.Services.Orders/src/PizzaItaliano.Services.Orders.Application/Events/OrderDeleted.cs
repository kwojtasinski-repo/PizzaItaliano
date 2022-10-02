using Convey.CQRS.Events;
using System;

namespace PizzaItaliano.Services.Orders.Application.Events
{
    [Contract]
    public class OrderDeleted : IEvent
    {
        public Guid OrderId { get; }

        public OrderDeleted(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}

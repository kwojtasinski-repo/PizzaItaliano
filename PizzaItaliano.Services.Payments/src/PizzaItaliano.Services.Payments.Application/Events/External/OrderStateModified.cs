using Convey.CQRS.Events;
using Convey.MessageBrokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Events.External
{
    [Message("order")] // binding do odpowiedniej wymiany
    public class OrderStateModified : IEvent
    {
        public Guid OrderId { get; }
        public OrderStatus OrderStatusBeforeChange { get; }
        public OrderStatus OrderStatusAfterChange { get; }

        public OrderStateModified(Guid orderId, OrderStatus orderStatusBeforeChange, OrderStatus orderStatusAfterChange)
        {
            OrderId = orderId;
            OrderStatusBeforeChange = orderStatusBeforeChange;
            OrderStatusAfterChange = orderStatusAfterChange;
        }
    }

    public enum OrderStatus
    {
        New,
        Ready,
        Paid,
        Released
    }
}

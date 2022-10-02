using Convey.CQRS.Events;
using Convey.MessageBrokers;
using System;

namespace PizzaItaliano.Services.Releases.Application.Events.External
{
    [Message("order")]
    public class OrderDeleted : IEvent
    {
        public Guid OrderId { get; }

        public OrderDeleted(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}

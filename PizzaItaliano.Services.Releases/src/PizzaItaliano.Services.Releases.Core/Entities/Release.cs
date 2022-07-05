using PizzaItaliano.Services.Releases.Core.Events;
using System;

namespace PizzaItaliano.Services.Releases.Core.Entities
{
    public class Release : AggregateRoot
    {
        public Guid OrderId { get; private set; }
        public Guid OrderProductId { get; private set; }
        public DateTime Date { get; private set; }
        public Guid UserId { get; set; }

        public Release(Guid id, Guid orderId, Guid orderProductId, DateTime date, Guid userId)
        {
            Id = id;
            OrderId = orderId;
            Date = date;
            OrderProductId = orderProductId;
            UserId = userId;
        }

        public static Release Create(Guid id, Guid orderId, Guid orderProductId, DateTime date, Guid userId)
        {
            var release = new Release(id, orderId, orderProductId, date, userId);
            release.AddEvent(new CreateRelease(release));
            return release;
        }
    }
}

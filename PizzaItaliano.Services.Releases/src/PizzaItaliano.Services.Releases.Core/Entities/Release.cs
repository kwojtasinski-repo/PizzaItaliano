using PizzaItaliano.Services.Releases.Core.Events;
using System;

namespace PizzaItaliano.Services.Releases.Core.Entities
{
    public class Release : AggregateRoot
    {
        public Guid OrderId { get; private set; }
        public DateTime Date { get; private set; }

        public Release(Guid id, Guid orderId, DateTime date)
        {
            Id = id;
            OrderId = orderId;
            Date = date;
        }

        public static Release Create(Guid id, Guid orderId, DateTime date)
        {
            var release = new Release(id, orderId, date);
            release.AddEvent(new CreateRelease(release));
            return release;
        }
    }
}

using Convey.CQRS.Events;
using PizzaItaliano.Services.Releases.Application.Events;
using PizzaItaliano.Services.Releases.Application.Services;
using PizzaItaliano.Services.Releases.Core;
using PizzaItaliano.Services.Releases.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Infrastructure.Services
{
    internal sealed class EventMapper : IEventMapper
    {
        public IEvent Map(IDomainEvent @event)
        {
            IEvent eventMapped = @event switch
            {
                CreateRelease e => new ReleaseAdded(e.Release.Id, e.Release.OrderId, e.Release.OrderProductId),
                _ => null
            };

            return eventMapped;
        }

        public IEnumerable<IEvent> MapAll(IEnumerable<IDomainEvent> events)
        {
            var eventsMapped = events.Select(e => Map(e));
            return eventsMapped;
        }
    }
}

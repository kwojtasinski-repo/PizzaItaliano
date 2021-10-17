using Convey.CQRS.Events;
using PizzaItaliano.Services.Products.Application.Events;
using PizzaItaliano.Services.Products.Application.Services;
using PizzaItaliano.Services.Products.Core;
using PizzaItaliano.Services.Products.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Infrastructure.Services
{
    internal sealed class EventMapper : IEventMapper
    {
        public IEvent Map(IDomainEvent @event)
        {
            IEvent eventMapped = @event switch
            {
                ProductAdded e => new ProductCreated(e.Product.Id),
                Core.Events.ProductModified e => new Application.Events.ProductModified(e.Product.Id),
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

using Convey.CQRS.Events;
using PizzaItaliano.Services.Orders.Application.Events;
using PizzaItaliano.Services.Orders.Application.Services;
using PizzaItaliano.Services.Orders.Core;
using PizzaItaliano.Services.Orders.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Infrastructure.Services
{
    internal sealed class EventMapper : IEventMapper
    {
        public IEvent Map(IDomainEvent @event)
        {
            IEvent eventMapped = @event switch
            {
                OrderCreated e => new OrderAdded(e.Order.Id),
                OrderProductAdd e => new OrderProductAdded(e.Order.Id, e.OrderProduct.Id, e.OrderProduct.ProductId),
                OrderStateChanged e => new OrderStateModified(e.OrderAfterChange.Id, e.OrderBeforeChange.OrderStatus, e.OrderAfterChange.OrderStatus),
                OrderProductRemoved e => new OrderProductDeleted(e.Order.Id, e.OrderProduct.Id),
                OrderProductStateChanged e => new OrderProductStateModified(e.OrderProductBeforeChange.Id, e.OrderProductBeforeChange.OrderProductStatus, e.OrderProductAfterChange.OrderProductStatus),

                _ => null
            };

            return eventMapped;
        }

        public IEnumerable<IEvent> MapAll(IEnumerable<IDomainEvent> events)
        {
            var mappedEvents = events.Select(e => Map(e));
            return mappedEvents;
        }
    }
}

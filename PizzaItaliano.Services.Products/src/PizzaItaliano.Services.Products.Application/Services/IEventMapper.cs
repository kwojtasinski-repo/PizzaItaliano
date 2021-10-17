using Convey.CQRS.Events;
using PizzaItaliano.Services.Products.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Services
{
    public interface IEventMapper
    {
        IEnumerable<IEvent> MapAll(IEnumerable<IDomainEvent> events);
        IEvent Map(IDomainEvent @event);
    }
}

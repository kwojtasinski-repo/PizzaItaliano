using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Services
{
    public interface IMessageBroker
    {
        Task PublishAsync(params IEvent[] events);
        Task PublishAsync(IEnumerable<IEvent> events);
    }
}

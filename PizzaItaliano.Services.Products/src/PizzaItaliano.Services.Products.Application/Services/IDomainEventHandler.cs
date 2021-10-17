using PizzaItaliano.Services.Products.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Services
{
    public interface IDomainEventHandler<in T> where T : class, IDomainEvent
    {
        Task HandleAsync(T @event);
    }
}

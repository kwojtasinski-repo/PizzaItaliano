using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Events
{
    [Contract]
    public class ProductCreated : IEvent
    {
        public Guid ProductId { get; }

        public ProductCreated(Guid productId)
        {
            ProductId = productId;
        }
    }
}

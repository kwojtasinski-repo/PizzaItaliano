using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Events
{
    public class ProductDeleted : IEvent
    {
        public Guid ProductId { get; }

        public ProductDeleted(Guid productId)
        {
            ProductId = productId;
        }
    }
}

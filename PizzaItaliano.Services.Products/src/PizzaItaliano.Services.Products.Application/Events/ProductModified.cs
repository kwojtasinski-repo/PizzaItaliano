using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Events
{
    [Contract]
    public class ProductModified : IEvent
    {
        public Guid ProductId { get; }

        public ProductModified(Guid productId)
        {
            ProductId = productId;
        }
    }
}

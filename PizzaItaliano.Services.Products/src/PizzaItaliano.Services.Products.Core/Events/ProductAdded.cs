using PizzaItaliano.Services.Products.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Core.Events
{
    public class ProductAdded : IDomainEvent
    {
        public Product Product { get; }

        public ProductAdded(Product product)
        {
            Product = product;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Core.Entities
{
    public class Product
    {
        public AggregateId Id { get; protected set; }
        public string Name { get; protected set; }
        public decimal Cost { get; protected set; }

        public Product(Guid id, string name, decimal cost)
        {
            Id = id;
            Name = name;
            Cost = cost;
        }

        public static Product Create(Guid id, string name, decimal cost)
        {
            var product = new Product(id, name, cost);
        }
    }
}

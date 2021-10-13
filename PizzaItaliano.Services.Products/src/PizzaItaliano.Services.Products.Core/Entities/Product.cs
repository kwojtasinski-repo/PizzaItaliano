using PizzaItaliano.Services.Products.Core.Events;
using PizzaItaliano.Services.Products.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Core.Entities
{
    public class Product : AggregateRoot
    {
        public string Name { get; private set; }
        public decimal Cost { get; private set; }
        public ProductStatus Status { get; private set; }
        public bool CanBeDelete => Status == ProductStatus.New;

        public Product(Guid id, string name, decimal cost, ProductStatus status)
        {
            ValidCost(cost);
            ValidName(name);
            Id = id;
            Name = name;
            Cost = cost;
            Status = status;
        }

        public static Product Create(Guid id, string name, decimal cost, ProductStatus status)
        {
            ValidCost(cost);
            ValidName(name);
            var product = new Product(id, name, cost, status);
            return product;
        }

        private static void ValidCost(decimal cost)
        {
            if (cost < 0)
            {
                throw new InvalidProductCostException(cost);
            }

        }

        private static void ValidName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ProductNameCannotBeEmptyException();
            }

        }
        
        public void Modified(string name, decimal cost)
        {
            ValidName(name);
            ValidCost(cost);
            Name = name;
            Cost = cost;
            AddEvent(new ProductModified(this));
        }

        public void Modified(string name)
        {
            ValidName(name);
            Name = name;
            AddEvent(new ProductModified(this));
        }

        public void Modified(decimal cost)
        {
            ValidCost(cost);
            Cost = cost;
            AddEvent(new ProductModified(this));
        }
    }
}

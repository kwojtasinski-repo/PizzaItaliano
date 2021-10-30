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
            ValidCost(id, cost);
            ValidName(name);
            Id = id;
            Name = name;
            Cost = cost;
            Status = status;
        }

        public static Product Create(Guid id, string name, decimal cost, ProductStatus status)
        {
            var product = new Product(id, name, cost, status);
            product.AddEvent(new ProductAdded(product));
            return product;
        }

        public void MarkAsUsed()
        {
            if (Status != ProductStatus.New)
            {
                throw new CannotChangeProductStatusException(Id, Status, ProductStatus.Used);
            }

            Status = ProductStatus.Used;
        }

        private static void ValidCost(Guid id, decimal cost)
        {
            if (cost < 0)
            {
                throw new InvalidProductCostException(id, cost);
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
            ValidCost(Id, cost);
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
            ValidCost(Id, cost);
            Cost = cost;
            AddEvent(new ProductModified(this));
        }
    }
}

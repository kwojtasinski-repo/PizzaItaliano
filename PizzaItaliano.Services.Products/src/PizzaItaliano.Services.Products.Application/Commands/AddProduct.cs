using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Commands
{
    public class AddProduct : ICommand
    {
        public Guid ProductId { get; }
        public string Name { get; }
        public decimal Cost { get; }

        public AddProduct(Guid productId, string name, decimal cost)
        {
            ProductId = productId;
            Name = name;
            Cost = cost;
        }
    }
}

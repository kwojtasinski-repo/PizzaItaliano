using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Core.Exceptions
{
    public class InvalidProductCostException : DomainException
    {
        public override string Code { get; } = "invalid_product_cost";
        public decimal Cost { get; }
        public Guid ProductId { get; }

        public InvalidProductCostException(Guid productId, decimal cost) : base($"Invalid cost: '{cost}' for product with id: '{productId}'")
        {
            ProductId = productId;
            Cost = cost;
        }
    }
}

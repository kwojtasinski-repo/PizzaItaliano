using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Exceptions
{
    public class InvalidProductCostException : AppException
    {
        public override string Code => "invalid_product_cost";
        public Guid ProductId { get; }

        public InvalidProductCostException(Guid productId) : base($"Invalid cost for product with id: '{productId}'")
        {
            ProductId = productId;
        }
    }
}

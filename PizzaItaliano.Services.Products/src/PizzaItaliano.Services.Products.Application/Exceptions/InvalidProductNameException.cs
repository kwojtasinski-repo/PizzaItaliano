using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Exceptions
{
    public class InvalidProductNameException : AppException
    {
        public override string Code => "invalid_product_name";
        public Guid ProductId { get; }

        public InvalidProductNameException(Guid productId) : base($"Invalid name for product with id: '{productId}'")
        {
            ProductId = productId;
        }
    }
}

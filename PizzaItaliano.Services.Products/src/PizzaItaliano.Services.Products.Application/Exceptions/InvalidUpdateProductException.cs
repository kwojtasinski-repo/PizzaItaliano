using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Exceptions
{
    public class InvalidUpdateProductException : AppException
    {
        public override string Code => "invalid_update_product";
        public Guid ProductId { get; }

        public InvalidUpdateProductException(Guid productId) : base($"Cannot update product with id: '{productId}'")
        {
            ProductId = productId;
        }
    }
}

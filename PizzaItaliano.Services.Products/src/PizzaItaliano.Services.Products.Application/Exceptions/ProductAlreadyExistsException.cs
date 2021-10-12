using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Exceptions
{
    public class ProductAlreadyExistsException : AppException
    {
        public override string Code { get; } = "product_already_exists";
        public Guid ProductId { get; set; }

        public ProductAlreadyExistsException(Guid productId) : base($"Product with id {productId} already exists.")
        {
            ProductId = productId;
        }
    }
}

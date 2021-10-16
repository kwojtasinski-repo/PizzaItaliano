using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Exceptions
{
    public class ProductNotFoundException : AppException
    {
        public override string Code => "product_not_found";
        public Guid ProductId { get; }

        public ProductNotFoundException(Guid productId) : base($"Product with id: '{productId}' was not found")
        {
            ProductId = productId;
        }
    }
}

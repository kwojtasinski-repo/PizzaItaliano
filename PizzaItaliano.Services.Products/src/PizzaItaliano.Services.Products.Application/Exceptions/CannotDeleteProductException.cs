using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Exceptions
{
    public class CannotDeleteProductException : AppException
    {
        public override string Code => "cannot_delete_product";
        public Guid ProductId { get; }

        public CannotDeleteProductException(Guid productId) : base($"Cannot delete product with id: {productId} ")
        {
            ProductId = productId;
        }
    }
}

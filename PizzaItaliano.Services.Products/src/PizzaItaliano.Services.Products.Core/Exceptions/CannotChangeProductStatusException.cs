using PizzaItaliano.Services.Products.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Core.Exceptions
{
    public class CannotChangeProductStatusException : DomainException
    {
        public override string Code => "cannot_change_product_status";
        public Guid ProductId { get; }
        public ProductStatus CurrentProductStatus { get; }
        public ProductStatus NewProductStatus { get; }

        public CannotChangeProductStatusException(Guid productId, ProductStatus currentProductStatus, ProductStatus newProductStatus) : base($"Cannot change status from '{currentProductStatus}' to '{newProductStatus}' for product with id: '{productId}'")
        {
            ProductId = productId;
            CurrentProductStatus = currentProductStatus;
            NewProductStatus = newProductStatus;
        }
    }
}

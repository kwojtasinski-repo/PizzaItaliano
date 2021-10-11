using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Core.Exceptions
{
    public class ProductNameCannotBeEmptyException : DomainException
    {
        public override string Code { get; } = "product_name_cannot_be_empty";

        public ProductNameCannotBeEmptyException() : base("Product name cannot be empty")
        {
        }
    }
}

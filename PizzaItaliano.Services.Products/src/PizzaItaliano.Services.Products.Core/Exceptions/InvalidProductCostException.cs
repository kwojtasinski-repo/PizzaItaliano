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


        public InvalidProductCostException(decimal cost) : base($"Invalid product cost: '{cost}'")
        {
            Cost = cost;
        }
    }
}

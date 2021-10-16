using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Exceptions
{
    public class InvalidOrderProductCostException : DomainException
    {
        public override string Code => "invalid_order_product_cost_exception";
        public Guid OrderProductId { get; }
        public decimal Cost { get; }

        public InvalidOrderProductCostException(Guid orderProductId, decimal cost) : base($"Invalid cost: '{cost}' for order product with id: '{orderProductId}'")
        {
            OrderProductId = orderProductId;
            Cost = cost;
        }
    }
}

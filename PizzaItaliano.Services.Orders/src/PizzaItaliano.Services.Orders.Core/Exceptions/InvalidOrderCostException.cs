using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Exceptions
{
    public class InvalidOrderCostException : DomainException
    {
        public override string Code => "invalid_order_cost";
        public Guid OrderId { get; }
        public decimal Cost { get; }

        public InvalidOrderCostException(Guid orderId, decimal cost) : base($"Invalid cost '{cost}' for order with id: '{orderId}'")
        {
            OrderId = orderId;
            Cost = cost;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Exceptions
{
    public class InvalidOrderNumberException : DomainException
    {
        public override string Code => "invalid_order_number";
        public Guid OrderId { get; }
        public string OrderNumber { get; }

        public InvalidOrderNumberException(Guid orderId, string orderNumber) : base($"Invalid order number '{orderNumber}' for order with id: '{orderId}'")
        {
            OrderId = orderId;
            OrderNumber = orderNumber;
        }
    }
}

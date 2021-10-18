using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Exceptions
{
    public class InvalidOrderProductQuantityException : DomainException
    {
        public override string Code => "invalid_order_product_quantity";
        public Guid OrderProductId { get; }
        public int Quantity { get; }

        public InvalidOrderProductQuantityException(Guid orderProductId, int quantity) : base($"Invalid quantity: {quantity} for order product with id: {orderProductId}")
        {
            OrderProductId = orderProductId;
            Quantity = quantity;
        }
    }
}

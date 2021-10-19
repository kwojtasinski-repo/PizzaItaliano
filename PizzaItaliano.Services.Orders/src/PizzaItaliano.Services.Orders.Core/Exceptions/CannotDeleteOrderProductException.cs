using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Exceptions
{
    public class CannotDeleteOrderProductException : DomainException
    {
        public override string Code => "cannot_delete_order_product";
        public Guid OrderId { get; }
        public Guid OrderProductId { get; }
        public int Quantity { get; }

        public CannotDeleteOrderProductException(Guid orderId, Guid orderProductId, int quantity) : base($"Cannot delete order product with id: '{orderProductId}' and '{quantity}' from order with id: {orderId}")
        {
            OrderId = orderId;
            OrderProductId = orderProductId;
            Quantity = quantity;
        }
    }
}

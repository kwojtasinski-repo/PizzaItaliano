using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Exceptions
{
    public class CannotChangeOrderProductStateException : DomainException
    {
        public override string Code => "cannot_change_order_product_state";
        public Guid OrderProductId { get; }
        public OrderProductStatus CurrentOrderProductStatus { get; }
        public OrderProductStatus NewOrderProductStatus { get; }

        public CannotChangeOrderProductStateException(Guid orderProductId, OrderProductStatus currentOrderProductStatus, OrderProductStatus newOrderProductStatus) : base($"OrderProduct with id: '{orderProductId}' cannot change status from '{currentOrderProductStatus}' to '{newOrderProductStatus}'")
        {
            OrderProductId = orderProductId;
            CurrentOrderProductStatus = currentOrderProductStatus;
            NewOrderProductStatus = newOrderProductStatus;
        }
    }
}

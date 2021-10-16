using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Exceptions
{
    public class OrderAlreadyExistsException : AppException
    {
        public override string Code => "order_already_exists";
        public Guid OrderId { get; }

        public OrderAlreadyExistsException(Guid orderId) : base($"Order with id: {orderId} already exists")
        {
            OrderId = orderId;
        }
    }
}

using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PizzaItaliano.Services.Orders.Application.Events
{
    [Contract]
    public class OrderProductDeleted : IEvent
    {
        public Guid OrderId { get; }
        public Guid OrderProductId { get; }

        public OrderProductDeleted(Guid orderId, Guid orderProductId)
        {
            OrderId = orderId;
            OrderProductId = orderProductId;
        }
    }
}

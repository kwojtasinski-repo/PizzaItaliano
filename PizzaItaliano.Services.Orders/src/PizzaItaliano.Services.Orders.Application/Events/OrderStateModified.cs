using Convey.CQRS.Events;
using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PizzaItaliano.Services.Orders.Application.Events
{
    [Contract]
    public class OrderStateModified : IEvent
    {
        public Guid OrderId { get; }
        public OrderStatus OrderStatusBeforeChange { get; }
        public OrderStatus OrderStatusAfterChange { get; }

        public OrderStateModified(Guid orderId, OrderStatus orderStatusBeforeChange, OrderStatus orderStatusAfterChange)
        {
            OrderId = orderId;
            OrderStatusBeforeChange = orderStatusBeforeChange;
            OrderStatusAfterChange = orderStatusAfterChange;
        }
    }
}

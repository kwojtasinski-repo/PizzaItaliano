using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Events
{
    public class OrderAdded : IEvent
    {
        public Guid OrderId { get; }

        public OrderAdded(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}

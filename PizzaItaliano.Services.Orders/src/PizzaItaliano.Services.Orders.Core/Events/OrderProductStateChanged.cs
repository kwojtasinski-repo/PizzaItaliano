using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Events
{
    public class OrderProductStateChanged : IDomainEvent
    {
        public OrderProduct OrderProductBeforeChange { get; }
        public OrderProduct OrderProductAfterChange { get; }

        public OrderProductStateChanged(OrderProduct orderProductBeforeChange, OrderProduct orderProductAfterChange)
        {
            OrderProductBeforeChange = orderProductBeforeChange;
            OrderProductAfterChange = orderProductAfterChange;
        }
    }
}

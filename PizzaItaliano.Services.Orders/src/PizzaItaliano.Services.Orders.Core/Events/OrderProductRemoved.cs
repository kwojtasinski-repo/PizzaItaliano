﻿using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Events
{
    public class OrderProductRemoved : IDomainEvent
    {
        public Order Order { get; }
        public OrderProduct OrderProduct { get; }

        public OrderProductRemoved(Order order, OrderProduct orderProduct)
        {
            Order = order;
            OrderProduct = orderProduct;
        }
    }
}

﻿using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PizzaItaliano.Services.Orders.Application.Events
{
    [Contract]
    public class OrderProductAdded : IEvent
    {
        public Guid OrderId { get; }
        public Guid OrderProductId { get; }
        public Guid ProductId { get; }

        public OrderProductAdded(Guid orderId, Guid orderProductId, Guid productId)
        {
            OrderId = orderId;
            OrderProductId = orderProductId;
            ProductId = productId;
        }
    }
}

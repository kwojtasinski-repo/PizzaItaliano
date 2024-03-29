﻿using Convey.CQRS.Events;
using Convey.MessageBrokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Events.External
{
    [Message("release")] // binding do odpowiedniej wymiany
    public class ReleaseAdded : IEvent
    {
        public Guid ReleaseId { get; }
        public Guid OrderId { get; }
        public Guid OrderProductId { get; }

        public ReleaseAdded(Guid releaseId, Guid orderId, Guid orderProductId)
        {
            ReleaseId = releaseId;
            OrderId = orderId;
            OrderProductId = orderProductId;
        }
    }
}

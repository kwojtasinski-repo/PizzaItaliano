using Convey.CQRS.Events;
using Convey.MessageBrokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Application.Events.External
{
	[Message("order")] // binding do odpowiedniej wymiany
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

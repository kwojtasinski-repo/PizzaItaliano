using Convey.CQRS.Commands;
using PizzaItaliano.Services.Orders.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Commands
{
    [Contract]
    public class DeleteOrderProduct : ICommand
    {
        public Guid OrderId { get; }
        public Guid OrderProductId { get; }
        [JsonConverter(typeof(StringToIntConverter))]
        public int Quantity { get; }

        public DeleteOrderProduct(Guid orderId, Guid orderProductId, int quantity)
        {
            OrderId = orderId;
            OrderProductId = orderProductId;
            Quantity = quantity;
        }
    }
}

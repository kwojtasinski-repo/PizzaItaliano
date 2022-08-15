using Convey.CQRS.Commands;
using System;

namespace PizzaItaliano.Services.Orders.Application.Commands
{
    [Contract]
    public class SetOrderStatusReady : ICommand
    {
        public Guid OrderId { get; set; }

        public SetOrderStatusReady(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}

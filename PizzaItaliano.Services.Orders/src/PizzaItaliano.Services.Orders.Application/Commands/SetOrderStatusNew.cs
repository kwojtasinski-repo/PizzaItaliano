using Convey.CQRS.Commands;
using System;

namespace PizzaItaliano.Services.Orders.Application.Commands
{
    [Contract]
    public class SetOrderStatusNew : ICommand
    {
        public Guid OrderId { get; set; }

        public SetOrderStatusNew(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}

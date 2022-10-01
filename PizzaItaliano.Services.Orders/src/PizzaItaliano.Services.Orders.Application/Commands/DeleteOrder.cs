using Convey.CQRS.Commands;
using System;

namespace PizzaItaliano.Services.Orders.Application.Commands
{
    [Contract]
    public class DeleteOrder : ICommand
    {
        public Guid OrderId { get; set; }
    }
}

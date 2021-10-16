using Convey.CQRS.Commands;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Core.Entities;
using PizzaItaliano.Services.Orders.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Commands.Handlers
{
    public class AddOrderHandler : ICommandHandler<AddOrder>
    {
        private readonly IOrderRepository _orderRepository;

        public AddOrderHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task HandleAsync(AddOrder command)
        {
            var exists = await _orderRepository.ExistsAsync(command.OrderId);
            if (exists)
            {
                throw new OrderAlreadyExistsException(command.OrderId);
            }

            var currentDate = DateTime.Now.Date;
            var lastOrderNumberToday = _orderRepository.GetCollection(o => o.OrderDate > currentDate).OrderByDescending(o => o.OrderDate).Select(o => o.OrderNumber).FirstOrDefault();
            int number = 1;
            if (lastOrderNumberToday is { })
            {
                var stringNumber = lastOrderNumberToday.Substring(15);//16
                int.TryParse(stringNumber, out number);
                number++;
            }

            var orderNumber = new StringBuilder("ORD/")
                .Append(currentDate.Year.ToString("YYYY")).Append("/").Append(currentDate.Month.ToString("MM"))
                .Append("/").Append(currentDate.Day.ToString("dd")).Append("/").Append(number).ToString();

            var order = Order.Create(command.OrderId, orderNumber, decimal.Zero);
        }
    }
}

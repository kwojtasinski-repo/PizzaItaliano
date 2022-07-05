using Convey.CQRS.Commands;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Application.Services;
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
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;
        private readonly IAppContext _appContext;

        public AddOrderHandler(IOrderRepository orderRepository, IMessageBroker messageBroker, IEventMapper eventMapper, IAppContext appContext)
        {
            _orderRepository = orderRepository;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
            _appContext = appContext;
        }

        public async Task HandleAsync(AddOrder command)
        {
            if (_appContext.Identity.Id == Guid.Empty)
            {
                throw new InvalidUserIdException(_appContext.Identity.Id);
            }

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
                .Append(currentDate.Year).Append("/").Append(currentDate.Month.ToString("d2"))
                .Append("/").Append(currentDate.Day.ToString("00")).Append("/").Append(number).ToString();

            var order = Order.Create(command.OrderId, orderNumber, decimal.Zero, _appContext.Identity.Id);

            await _orderRepository.AddAsync(order);
            var integrationEvents = _eventMapper.MapAll(order.Events);
            await _messageBroker.PublishAsync(integrationEvents);
        }
    }
}

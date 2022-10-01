using Convey.CQRS.Commands;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Application.Services;
using PizzaItaliano.Services.Orders.Core.Entities;
using PizzaItaliano.Services.Orders.Core.Repositories;
using System;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Commands.Handlers
{
    internal class SetOrderStatusNewHandler : ICommandHandler<SetOrderStatusNew>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public SetOrderStatusNewHandler(IOrderRepository orderRepository, IMessageBroker messageBroker,
            IEventMapper eventMapper)
        {
            _orderRepository = orderRepository;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
        }

        public async Task HandleAsync(SetOrderStatusNew command)
        {
            var order = await _orderRepository.GetAsync(command.OrderId);
            if (order is null)
            {
                throw new OrderNotFoundException(command.OrderId);
            }

            if (order.OrderStatus == OrderStatus.Ready)
            {
                throw new CannotChangeOrderStatusException(order.Id);
            }

            order.OrderNew();
            await _orderRepository.UpdateAsync(order);
            var events = _eventMapper.MapAll(order.Events);
            await _messageBroker.PublishAsync(events);
        }
    }
}

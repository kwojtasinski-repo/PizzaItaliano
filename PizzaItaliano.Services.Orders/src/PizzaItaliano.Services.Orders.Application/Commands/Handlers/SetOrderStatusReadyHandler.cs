using Convey.CQRS.Commands;
using PizzaItaliano.Services.Orders.Application.Services;
using PizzaItaliano.Services.Orders.Core.Repositories;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Convey.MessageBrokers;

namespace PizzaItaliano.Services.Orders.Application.Commands.Handlers
{
    public class SetOrderStatusReadyHandler : ICommandHandler<SetOrderStatusReady>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;
        private readonly IAppContext _appContext;

        public SetOrderStatusReadyHandler(IOrderRepository orderRepository, IMessageBroker messageBroker, IEventMapper eventMapper,
            ICorrelationContextAccessor correlationContextAccessor, IAppContext appContext)
        {
            _orderRepository = orderRepository;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
            _correlationContextAccessor = correlationContextAccessor;
            _appContext = appContext;
        }

        public async Task HandleAsync(SetOrderStatusReady command)
        {
            var order = await _orderRepository.GetAsync(command.OrderId);
            if (order is null)
            {
                throw new OrderNotFoundException(command.OrderId);
            }

            if (order.OrderStatus == Core.Entities.OrderStatus.Released)
            {
                throw new CannotChangeOrderStatusException(order.Id);
            }

            order.OrderReady();
            await _orderRepository.UpdateAsync(order);
            _correlationContextAccessor.CorrelationContext = new { user = new { Id = _appContext.Identity.Id } };
            var events = _eventMapper.MapAll(order.Events);
            await _messageBroker.PublishAsync(events);
        }
    }
}

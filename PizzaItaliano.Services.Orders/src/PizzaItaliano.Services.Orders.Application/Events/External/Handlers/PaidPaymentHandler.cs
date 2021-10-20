using Convey.CQRS.Events;
using PizzaItaliano.Services.Orders.Application.Services;
using PizzaItaliano.Services.Orders.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Events.External.Handlers
{
    public class PaidPaymentHandler : IEventHandler<PaidPayment>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public PaidPaymentHandler(IOrderRepository orderRepository, IMessageBroker messageBroker, IEventMapper eventMapper)
        {
            _orderRepository = orderRepository;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
        }

        public async Task HandleAsync(PaidPayment @event)
        {
            var order = await _orderRepository.GetAsync(@event.OrderId);
            if (order is null)
            {
                return;
            }

            if (order.OrderStatus != Core.Entities.OrderStatus.Ready)
            {
                return;
            }

            order.OrderPaid();
            await _orderRepository.UpdateAsync(order);
            var integratedEvents = _eventMapper.MapAll(order.Events);
            await _messageBroker.PublishAsync(integratedEvents);
        }
    }
}

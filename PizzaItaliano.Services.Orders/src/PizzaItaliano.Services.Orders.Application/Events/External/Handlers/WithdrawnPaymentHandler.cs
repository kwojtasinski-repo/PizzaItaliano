using Convey.CQRS.Events;
using PizzaItaliano.Services.Orders.Application.Services;
using PizzaItaliano.Services.Orders.Core.Repositories;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Events.External.Handlers
{
    internal class WithdrawnPaymentHandler : IEventHandler<WithdrawnPayment>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public WithdrawnPaymentHandler(IOrderRepository orderRepository, IMessageBroker messageBroker, IEventMapper eventMapper)
        {
            _orderRepository = orderRepository;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
        }

        public async Task HandleAsync(WithdrawnPayment @event)
        {
            var order = await _orderRepository.GetAsync(@event.OrderId);
            if (order is null)
            {
                return;
            }

            if (order.OrderStatus == Core.Entities.OrderStatus.Ready)
            {
                return;
            }

            order.OrderReady();
            await _orderRepository.UpdateAsync(order);
            var integratedEvents = _eventMapper.MapAll(order.Events);
            await _messageBroker.PublishAsync(integratedEvents);
        }
    }
}

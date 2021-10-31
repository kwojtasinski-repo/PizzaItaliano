using Convey.CQRS.Events;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Application.Services;
using PizzaItaliano.Services.Orders.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Events.External.Handlers
{
    public class ReleaseAddedHandler : IEventHandler<ReleaseAdded>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public ReleaseAddedHandler(IOrderRepository orderRepository, IMessageBroker messageBroker, IEventMapper eventMapper)
        {
            _orderRepository = orderRepository;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
        }

        public async Task HandleAsync(ReleaseAdded @event)
        {
            var order = await _orderRepository.GetAsync(@event.OrderId);
            if (order is null)
            {
                return;
            }

            if (order.OrderStatus != Core.Entities.OrderStatus.Paid)
            {
                return;
            }

            var orderProducts = order.OrderProducts;
            if (orderProducts == null)
            {
                throw new CannotChangeOrderStatusException(order.Id);
            }
            else if (orderProducts.Count() == 0)
            {
                throw new CannotChangeOrderStatusException(order.Id);
            }

            var orderProduct = orderProducts.Where(op => op.Id == @event.OrderProductId).FirstOrDefault();
            orderProduct.OrderProductReleased();

            var allReleased = orderProducts.All(op => op.OrderProductStatus == Core.Entities.OrderProductStatus.Released);
            if (allReleased)
            {
                order.OrderReleased();
            }

            await _orderRepository.UpdateAsync(order);
            var events = _eventMapper.MapAll(orderProduct.Events);
            await _messageBroker.PublishAsync(events);

            if (allReleased)
            {
                var integratedEvents = _eventMapper.MapAll(order.Events);
                await _messageBroker.PublishAsync(integratedEvents);
            }
        }
    }
}

using Convey.CQRS.Commands;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Application.Services;
using PizzaItaliano.Services.Orders.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Commands.Handlers
{
    public class DeleteOrderProductHandler : ICommandHandler<DeleteOrderProduct>
    {
        private readonly IOrderRepository _orderRepository;
        //private readonly IProductServiceClient _productServiceClient;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public DeleteOrderProductHandler(IOrderRepository orderRepository, IMessageBroker messageBroker, IEventMapper eventMapper)
        {
            _orderRepository = orderRepository;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
        }
        public async Task HandleAsync(DeleteOrderProduct command)
        {
            if (command.Quantity <= 0)
            {
                throw new CannotDeleteOrderProductException(command.OrderId, command.OrderProductId, command.Quantity);
            }

            var order = await _orderRepository.GetAsync(command.OrderId);
            if (order is null)
            {
                throw new OrderNotFoundException(command.OrderId);
            }

            var orderProduct = order.OrderProducts.Where(op => op.Id == command.OrderProductId).FirstOrDefault();
            if (orderProduct is null)
            {
                throw new OrderProductNotFoundException(command.OrderProductId);
            }

            order.DeleteOrderProduct(orderProduct, command.Quantity);

            await _orderRepository.UpdateAsync(order);
            var integrationEvents = _eventMapper.MapAll(order.Events);
            await _messageBroker.PublishAsync(integrationEvents);
        }
    }
}

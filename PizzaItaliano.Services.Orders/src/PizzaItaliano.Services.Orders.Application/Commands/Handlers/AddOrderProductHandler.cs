using Convey.CQRS.Commands;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Application.Services;
using PizzaItaliano.Services.Orders.Application.Services.Clients;
using PizzaItaliano.Services.Orders.Core.Entities;
using PizzaItaliano.Services.Orders.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Commands.Handlers
{
    public class AddOrderProductHandler : ICommandHandler<AddOrderProduct>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductServiceClient _productServiceClient;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public AddOrderProductHandler(IOrderRepository orderRepository, IProductServiceClient productServiceClient, IMessageBroker messageBroker, IEventMapper eventMapper)
        {
            _orderRepository = orderRepository;
            _productServiceClient = productServiceClient;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
        }

        public async Task HandleAsync(AddOrderProduct command)
        {
            var order = await _orderRepository.GetAsync(command.OrderId);
            if (order is null)
            {
                throw new OrderNotFoundException(command.OrderId);
            }

            var product = await _productServiceClient.GetAsync(command.ProductId);
            if (product is null)
            {
                throw new ProductNotFoundException(command.ProductId);
            }

            var orderProduct = OrderProduct.Create(command.OrderProductId, command.Quantity, product.Cost, command.OrderId, command.ProductId);
            order.AddOrderProduct(orderProduct);
            var cost = product.Cost * command.Quantity;
            order.UpdateCost(cost);

            await _orderRepository.UpdateAsync(order);
            var integrationEvents = _eventMapper.MapAll(order.Events);
            await _messageBroker.PublishAsync(integrationEvents);
        }
    }
}

using Convey.CQRS.Commands;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Core.Entities;
using PizzaItaliano.Services.Orders.Core.Repositories;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Commands.Handlers
{
    internal class DeleteOrderHandler : ICommandHandler<DeleteOrder>
    {
        private readonly IOrderRepository _orderRepository;

        public DeleteOrderHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task HandleAsync(DeleteOrder command)
        {
            var order = await _orderRepository.GetAsync(command.OrderId);
            
            if (order == null)
            {
                throw new OrderNotFoundException(command.OrderId);
            }

            if (order.OrderStatus != OrderStatus.New)
            {
                throw new CannotDeleteOrderException(order.Id, order.OrderStatus);
            }

            await _orderRepository.DeleteAsync(order.Id);
        }
    }
}

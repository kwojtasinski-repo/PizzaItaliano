using Convey.CQRS.Events;
using PizzaItaliano.Services.Payments.Application.Exceptions;
using PizzaItaliano.Services.Payments.Core.Repositories;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Events.External.Handlers
{
    public class OrderDeletedHandler : IEventHandler<OrderDeleted>
    {
        private readonly IPaymentRepository _paymentRepository;

        public OrderDeletedHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task HandleAsync(OrderDeleted @event)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(@event.OrderId);

            if (payment == null)
            {
                return;
            }

            await _paymentRepository.DeleteAsync(payment.Id);
        }
    }
}

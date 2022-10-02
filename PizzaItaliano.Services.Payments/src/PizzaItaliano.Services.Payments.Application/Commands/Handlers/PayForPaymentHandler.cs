using Convey.CQRS.Commands;
using PizzaItaliano.Services.Payments.Application.Exceptions;
using PizzaItaliano.Services.Payments.Application.Services;
using PizzaItaliano.Services.Payments.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Commands.Handlers
{
    public class PayForPaymentHandler : ICommandHandler<PayForPayment>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public PayForPaymentHandler(IPaymentRepository paymentRepository, IMessageBroker messageBroker, IEventMapper eventMapper)
        {
            _paymentRepository = paymentRepository;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
        }

        public async Task HandleAsync(PayForPayment command)
        {
            if (command.OrderId == Guid.Empty)
            {
                throw new InvalidOrderIdException(command.OrderId);
            }

            var payment = await _paymentRepository.GetByOrderIdAsync(command.OrderId);
            if (payment is null)
            {
                throw new PaymentForOrderNotFoundException(command.OrderId);
            }

            if (payment.PaymentStatus == Core.Entities.PaymentStatus.Paid)
            {
                throw new CannotUpdatePaymentStatusException(payment.Id);
            }

            payment.MarkAsPaid();
            await _paymentRepository.UpdateAsync(payment);
            var integrationEvents = _eventMapper.MapAll(payment.Events);
            await _messageBroker.PublishAsync(integrationEvents);
        }
    }
}

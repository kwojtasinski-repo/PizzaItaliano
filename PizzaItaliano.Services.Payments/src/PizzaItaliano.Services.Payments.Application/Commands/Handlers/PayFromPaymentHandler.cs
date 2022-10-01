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
    public class PayFromPaymentHandler : ICommandHandler<PayFromPayment>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public PayFromPaymentHandler(IPaymentRepository paymentRepository, IMessageBroker messageBroker, IEventMapper eventMapper)
        {
            _paymentRepository = paymentRepository;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
        }

        public async Task HandleAsync(PayFromPayment command)
        {
            if (command.PaymentId == Guid.Empty)
            {
                throw new InvalidPaymentIdException(command.PaymentId);
            }

            var payment = await _paymentRepository.GetAsync(command.PaymentId);
            if (payment is null)
            {
                throw new PaymentNotFoundException(command.PaymentId);
            }

            if (payment.PaymentStatus == Core.Entities.PaymentStatus.Paid)
            {
                throw new CannotUpdatePaymentStatusException(command.PaymentId);
            }

            payment.MarkAsPaid();
            await _paymentRepository.UpdateAsync(payment);
            var integrationEvents = _eventMapper.MapAll(payment.Events);
            await _messageBroker.PublishAsync(integrationEvents);
        }
    }
}

using Convey.CQRS.Commands;
using PizzaItaliano.Services.Payments.Application.Exceptions;
using PizzaItaliano.Services.Payments.Application.Services;
using PizzaItaliano.Services.Payments.Core.Entities;
using PizzaItaliano.Services.Payments.Core.Repositories;
using System;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Commands.Handlers
{
    public class UpdatePaymentHandler : ICommandHandler<UpdatePayment>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public UpdatePaymentHandler(IPaymentRepository paymentRepository, IMessageBroker messageBroker, IEventMapper eventMapper)
        {
            _paymentRepository = paymentRepository;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
        }

        public async Task HandleAsync(UpdatePayment command)
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

            switch(command.PaymentStatus)
            {
                case PaymentStatus.Unpaid:
                    payment.MarkAsUnpaid();
                    break;
                case PaymentStatus.Paid:
                    payment.MarkAsPaid();
                    break;
                default:
                    throw new InvalidOperationException($"Invalid state of payment with id '{command.PaymentId}'");
            }

            await _paymentRepository.UpdateAsync(payment);
            var integrationEvents = _eventMapper.MapAll(payment.Events);
            await _messageBroker.PublishAsync(integrationEvents);
        }
    }
}

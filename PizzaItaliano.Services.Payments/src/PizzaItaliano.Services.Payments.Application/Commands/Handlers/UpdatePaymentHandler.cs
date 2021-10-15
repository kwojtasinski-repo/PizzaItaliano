using Convey.CQRS.Commands;
using PizzaItaliano.Services.Payments.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Commands.Handlers
{
    public class UpdatePaymentHandler : ICommandHandler<UpdatePayment>
    {
        private readonly IPaymentRepository _paymentRepository;

        public UpdatePaymentHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task HandleAsync(UpdatePayment command)
        {
            if (command.PaymentId == Guid.Empty)
            {
                return;
            }

            var payment = await _paymentRepository.GetAsync(command.PaymentId);

            if (payment is null)
            {
                return;
            }

            if (payment.PaymentStatus == Core.Entities.PaymentStatus.Paid)
            {
                return;
            }

            payment.MarkAsPaid();
            await _paymentRepository.UpdateAsync(payment);
        }
    }
}

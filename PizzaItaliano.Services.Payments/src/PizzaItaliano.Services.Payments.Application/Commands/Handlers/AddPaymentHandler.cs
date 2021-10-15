using Convey.CQRS.Commands;
using PizzaItaliano.Services.Payments.Application.Exceptions;
using PizzaItaliano.Services.Payments.Core.Entities;
using PizzaItaliano.Services.Payments.Core.Repositories;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Commands.Handlers
{
    public class AddPaymentHandler : ICommandHandler<AddPayment>
    {
        private readonly IPaymentRepository _paymentRepository;

        public AddPaymentHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task HandleAsync(AddPayment command)
        {
            if (command.OrderId == Guid.Empty)
            {
                throw new InvalidOrderIdException(command.PaymentId);
            }

            if (command.Cost < 0)
            {
                throw new InvalidCostException(command.Cost);
            }

            var exists = await _paymentRepository.ExistsAsync(command.PaymentId);

            if (exists)
            {
                throw new PaymentAlreadyExistsException(command.PaymentId);
            }

            var currentDate = DateTime.Now.Date;
            var lastPaymentNumberToday = _paymentRepository.GetCollection(p => p.CreateDate > currentDate).OrderByDescending(p => p.CreateDate).Select(p => p.Number).FirstOrDefault();
            int number = 1;
            if (lastPaymentNumberToday is { })
            {
                var stringNumber = lastPaymentNumberToday.Substring(15);//16
                int.TryParse(stringNumber, out number);
                number++;
            }

            var paymentNumber = new StringBuilder("PAY/")
                .Append(currentDate.Year.ToString("YYYY")).Append("/").Append(currentDate.Month.ToString("MM"))
                .Append("/").Append(currentDate.Day.ToString("dd")).Append("/").Append(number).ToString();

            var payment = Payment.Create(command.PaymentId, paymentNumber, command.Cost, command.OrderId, PaymentStatus.Paid);

            await _paymentRepository.AddAsync(payment);
        }
    }
}

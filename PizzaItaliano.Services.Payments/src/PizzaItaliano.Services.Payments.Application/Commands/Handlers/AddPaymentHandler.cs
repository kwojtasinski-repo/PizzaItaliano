using Convey.CQRS.Commands;
using PizzaItaliano.Services.Payments.Application.Exceptions;
using PizzaItaliano.Services.Payments.Application.Services;
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
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public AddPaymentHandler(IPaymentRepository paymentRepository, IMessageBroker messageBroker, IEventMapper eventMapper)
        {
            _paymentRepository = paymentRepository;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
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
            var lastPaymentNumberToday = _paymentRepository.GetCollection(p => p.CreateDate > currentDate).OrderByDescending(p => p.CreateDate).Select(p => p.PaymentNumber).FirstOrDefault();
            int number = 1;
            if (lastPaymentNumberToday is { })
            {
                var stringNumber = lastPaymentNumberToday.Substring(15);//16
                int.TryParse(stringNumber, out number);
                number++;
            }

            var paymentNumber = new StringBuilder("PAY/")
                .Append(currentDate.Year).Append("/").Append(currentDate.Month)
                .Append("/").Append(currentDate.Day).Append("/").Append(number).ToString();

            var payment = Payment.Create(command.PaymentId, paymentNumber, command.Cost, command.OrderId);

            await _paymentRepository.AddAsync(payment);
            var integrationEvents = _eventMapper.MapAll(payment.Events);
            await _messageBroker.PublishAsync(integrationEvents);
        }
    }
}

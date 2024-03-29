﻿using Convey.CQRS.Commands;
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
        private readonly IAppContext _appContext;

        public AddPaymentHandler(IPaymentRepository paymentRepository, IMessageBroker messageBroker, IEventMapper eventMapper, IAppContext appContext)
        {
            _paymentRepository = paymentRepository;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
            _appContext = appContext;
        }

        public async Task HandleAsync(AddPayment command)
        {
            if (_appContext.Identity.Id == Guid.Empty)
            {
                throw new InvalidUserIdException(_appContext.Identity.Id);
            }

            if (command.OrderId == Guid.Empty)
            {
                throw new InvalidOrderIdException(command.PaymentId);
            }

            if (command.Cost < 0)
            {
                throw new InvalidCostException(command.PaymentId, command.Cost);
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
                .Append(currentDate.Year).Append("/").Append(currentDate.Month.ToString("d2"))
                .Append("/").Append(currentDate.Day.ToString("00")).Append("/").Append(number).ToString();

            var payment = Payment.Create(command.PaymentId, paymentNumber, command.Cost, command.OrderId, _appContext.Identity.Id);

            await _paymentRepository.AddAsync(payment);
            var integrationEvents = _eventMapper.MapAll(payment.Events);
            await _messageBroker.PublishAsync(integrationEvents);
        }
    }
}

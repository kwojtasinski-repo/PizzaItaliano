using PizzaItaliano.Services.Payments.Application.Exceptions;
using Convey.MessageBrokers.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaItaliano.Services.Payments.Application.Commands;
using PizzaItaliano.Services.Payments.Application.Events.Rejected;
using PizzaItaliano.Services.Payments.Core.Exceptions;

namespace PizzaItaliano.Services.Payments.Infrastructure.Exceptions
{
    internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
    {
        public object Map(Exception exception, object message) // mapowanie wyjatkow na message (wiadomosci sa wysylane do RabbitMQ)
            => exception switch
            {
                // App Exceptions
                CannotUpdatePaymentStatusException ex => message switch
                { 
                    UpdatePayment command => new UpdatePaymentRejected(command.PaymentId, ex.Message, ex.Code),
                    _ => null
                },
                InvalidCostException ex => message switch
                {
                    AddPayment command => new AddPaymentRejected(command.PaymentId, ex.Message, ex.Code),
                    _ => null
                },
                InvalidOrderIdException ex => message switch
                {
                    AddPayment command => new AddPaymentRejected(command.PaymentId, ex.Message, ex.Code),
                    _ => null
                },
                InvalidPaymentIdException ex => message switch
                {
                    UpdatePayment command => new UpdatePaymentRejected(command.PaymentId, ex.Message, ex.Code),
                    _ => null
                },
                PaymentAlreadyExistsException ex => message switch
                {
                    AddPayment command => new AddPaymentRejected(command.PaymentId, ex.Message, ex.Code),
                    _ => null
                },
                PaymentNotFoundException ex => message switch
                {
                    UpdatePayment command => new UpdatePaymentRejected(command.PaymentId, ex.Message, ex.Code),
                    _ => null
                },

                // Domain Exceptions
                InvalidPaymentCostException ex => new AddPaymentRejected(Guid.Empty, ex.Message, ex.Code),

                // other unforeseen exceptions 
                _ => null
            };
    }
}

﻿using Convey.Logging.CQRS;
using PizzaItaliano.Services.Payments.Application.Commands;
using PizzaItaliano.Services.Payments.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Infrastructure.Logging
{
    internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
    {
        public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
        {
            var key = message.GetType();

            return Templates.TryGetValue(key, out var template) ? template : null;
        }

        private static readonly IReadOnlyDictionary<Type, HandlerLogTemplate> Templates = new Dictionary<Type, HandlerLogTemplate>()
        {
            [typeof(AddPayment)] = new HandlerLogTemplate
            {
                Before = "Adding an payment with id: {PaymentId}",
                After = "Added an payment with id: {PaymentId}",
                OnError = new Dictionary<Type, string> // w zaleznosci w jakim kontekscie poleci wyjatek
                {
                    [typeof(PaymentAlreadyExistsException)] = "Payment with id: {PaymentId} already exists",
                    [typeof(InvalidOrderIdException)] = "Invalid order id for payment with id: {PaymentId}",
                    [typeof(InvalidCostException)] = "Invalid cost for payment with id: {PaymentId}"
                }
            },
            [typeof(PayForPayment)] = new HandlerLogTemplate
            {
                Before = "Updating an payment for order with id: {OrderId}",
                After = "Updated an payment for order with id: {OrderId}",
                OnError = new Dictionary<Type, string>
                {
                    [typeof(InvalidPaymentIdException)] = "Invalid order id",
                    [typeof(PaymentNotFoundException)] = "Payment for order with id: {OrderId} not found",
                    [typeof(CannotUpdatePaymentStatusException)] = "Cannot update payment for order with id: {OrderId}"
                }
            }
        };
    }
}

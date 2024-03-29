﻿using Convey.CQRS.Commands;
using PizzaItaliano.Services.Payments.Core.Entities;
using System;

namespace PizzaItaliano.Services.Payments.Application.Commands
{
    [Contract]
    public class UpdatePayment : ICommand
    {
        public Guid PaymentId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}

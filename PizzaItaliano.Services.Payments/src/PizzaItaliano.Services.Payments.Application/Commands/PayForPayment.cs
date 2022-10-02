﻿using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Commands
{
    [Contract]
    public class PayForPayment : ICommand
    {
        public Guid OrderId { get; set; }
    }
}

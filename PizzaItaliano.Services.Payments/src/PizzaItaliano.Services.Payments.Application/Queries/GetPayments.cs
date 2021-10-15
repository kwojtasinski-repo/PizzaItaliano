using Convey.CQRS.Queries;
using PizzaItaliano.Services.Payments.Application.DTO;
using PizzaItaliano.Services.Payments.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Queries
{
    public class GetPayments : IQuery<IEnumerable<PaymentDto>>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}

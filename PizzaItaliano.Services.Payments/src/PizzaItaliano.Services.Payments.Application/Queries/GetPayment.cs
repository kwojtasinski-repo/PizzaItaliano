using Convey.CQRS.Queries;
using PizzaItaliano.Services.Payments.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Queries
{
    public class GetPayment : IQuery<PaymentDto>
    {
        public Guid PaymentId { get; set; }
    }
}

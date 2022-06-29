using Convey.CQRS.Queries;
using PizzaItaliano.Services.Payments.Application.DTO;
using System;

namespace PizzaItaliano.Services.Payments.Application.Queries
{
    public class GetPaymentByOrderId : IQuery<PaymentDto>
    {
        public Guid OrderId { get; set; }
    }
}

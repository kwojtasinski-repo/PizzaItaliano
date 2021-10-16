using PizzaItaliano.Services.Payments.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.DTO
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public string PaymentNumber { get; set; }
        public decimal Cost { get; set; }
        public Guid OrderId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public bool Paid => PaymentStatus == PaymentStatus.Paid;
    }
}

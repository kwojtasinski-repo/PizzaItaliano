using PizzaItaliano.Services.Payments.Core.Events;
using PizzaItaliano.Services.Payments.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Core.Entities
{
    public class Payment : AggregateRoot
    {
        public string PaymentNumber { get; private set; }
        public decimal Cost { get; private set; }
        public Guid OrderId { get; private set; }
        public DateTime CreateDate { get; private set; }
        public DateTime ModifiedDate { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }
        public Guid UserId { get; private set; }

        public Payment()
        {

        }

        public Payment(Guid id, string paymentNumber, decimal cost, Guid orderId, DateTime createDate, DateTime modifiedDate, PaymentStatus paymentStatus, Guid userId)
        {
            ValidPaymentNumber(id, paymentNumber);
            ValidCost(id, cost);
            Id = id;
            PaymentNumber = paymentNumber;
            Cost = cost;
            OrderId = orderId;
            CreateDate = createDate;
            ModifiedDate = modifiedDate;
            PaymentStatus = paymentStatus;
            UserId = userId;
        }

        public static Payment Create(Guid id, string number, decimal cost, Guid orderId, PaymentStatus paymentStatus, Guid userId)
        {
            var payment = new Payment(id, number, cost, orderId, DateTime.Now, DateTime.Now, paymentStatus, userId);
            payment.AddEvent(new CreatePayment(payment));
            return payment;
        }

        public static Payment Create(Guid id, string number, decimal cost, Guid orderId, Guid userId)
        {
            var payment = new Payment(id, number, cost, orderId, DateTime.Now, DateTime.Now, PaymentStatus.Unpaid, userId);
            payment.AddEvent(new CreatePayment(payment));
            return payment;
        }

        public void MarkAsPaid()
        {
            if (PaymentStatus == PaymentStatus.Paid)
            {
                return;
            }

            PaymentStatus = PaymentStatus.Paid;
            ModifiedDate = DateTime.Now;
            AddEvent(new PaymentPaid(this));
        }

        private static void ValidCost(Guid id, decimal cost)
        {
            if (cost < 0)
            {
                throw new InvalidPaymentCostException(id, cost);
            }
        }

        private static void ValidPaymentNumber(Guid id, string paymentNumber)
        {
            if (string.IsNullOrWhiteSpace(paymentNumber))
            {
                throw new InvalidPaymentNumberException(id, paymentNumber);
            }
        }
    }
}

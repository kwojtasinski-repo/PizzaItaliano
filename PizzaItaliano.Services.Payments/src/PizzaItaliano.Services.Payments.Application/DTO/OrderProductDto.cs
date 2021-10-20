using System;

namespace PizzaItaliano.Services.Payments.Application.DTO
{
    public class OrderProductDto
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public OrderProductStatus OrderProductStatus { get; set; }
    }

    public enum OrderProductStatus
    {
        New,
        Paid,
        Released
    }
}
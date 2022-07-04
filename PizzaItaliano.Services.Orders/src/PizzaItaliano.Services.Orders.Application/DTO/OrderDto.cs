using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;

namespace PizzaItaliano.Services.Orders.Application.DTO
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public decimal Cost { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public Guid UserId { get; set; }
        public IEnumerable<OrderProductDto> OrderProducts { get; set; }
    }
}

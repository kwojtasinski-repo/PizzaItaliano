using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.DTO
{
    public class OrderProductDto
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public OrderProductStatus OrderProductStatus { get; set; }
    }
}

using PizzaItaliano.Services.Payments.Application.Events.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.DTO
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public decimal Cost { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public IEnumerable<OrderProductDto> OrderProducts { get; set; }
    }
}

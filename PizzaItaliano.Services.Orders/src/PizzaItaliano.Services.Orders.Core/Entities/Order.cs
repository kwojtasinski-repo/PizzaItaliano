using PizzaItaliano.Services.Orders.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Entities
{
    public class Order : AggregateRoot
    {
        public string OrderNumber { get; private set; }
        public decimal Cost { get; private set; }
        public bool Paid { get; private set; }
        public OrderStatus OrderStatus { get; private set; }
        public DateTime OrderDate { get; private set; }
        public DateTime? ReleaseDate { get; private set; }

        public Order(Guid id, string orderNumber, decimal cost, bool paid, OrderStatus orderStatus, DateTime orderDate, DateTime? releaseDate)
        {
            Id = id;
            OrderNumber = orderNumber;
            Cost = cost;
            Paid = paid;
            OrderStatus = orderStatus;
            OrderDate = orderDate;
            ReleaseDate = releaseDate;
        }

        public static Order Create(Guid id, string orderNumber, decimal cost, bool paid, OrderStatus orderStatus)
        {
            var order = new Order(id, orderNumber, cost, paid, orderStatus, DateTime.Now, null);
            order.AddEvent(new CreateOrder(order));
            return order;
        }
    }
}

using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Exceptions
{
    public class CannotUpdateOrderCostException : DomainException
    {
        public override string Code => "cannot_update_order_cost";
        public Guid Id { get; }
        public decimal Cost { get; }
        public OrderStatus OrderStatus { get; }

        public CannotUpdateOrderCostException(Guid id, decimal cost, OrderStatus orderStatus) : base($"Cannot update cost by '{cost}' for order with id: '{id}' with status '{orderStatus}'")
        {
            Id = id;
            Cost = cost;
            OrderStatus = orderStatus;
        }
    }
}

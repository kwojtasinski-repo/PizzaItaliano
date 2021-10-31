using PizzaItaliano.Services.Orders.Core.Events;
using PizzaItaliano.Services.Orders.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Entities
{
    public class OrderProduct : AggregateRoot, IEquatable<OrderProduct>
    {
        public int Quantity { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public decimal Cost { get; private set; }
        public OrderProductStatus OrderProductStatus { get; private set; }

        public OrderProduct(Guid id, int quantity, decimal cost, Guid orderId, Guid productId, OrderProductStatus orderProductStatus)
        {
            ValidCost(id, cost);
            ValidQuantity(id, quantity);
            Id = id;
            Quantity = quantity;
            Cost = cost;
            OrderId = orderId;
            ProductId = productId;
            OrderProductStatus = orderProductStatus;
        }

        public static OrderProduct Create(Guid id, int quantity, decimal cost, Guid orderId, Guid productId)
        {
            var orderProduct = new OrderProduct(id, quantity, cost, orderId, productId, OrderProductStatus.New);
            return orderProduct;
        }

        public void OrderProductPaid()
        {
            if (OrderProductStatus != OrderProductStatus.New)
            {
                throw new CannotChangeOrderProductStateException(Id, OrderProductStatus, OrderProductStatus.Paid);
            }

            var orderProductBeforeChange = new OrderProduct(Id, Quantity, Cost, OrderId, ProductId, OrderProductStatus);
            OrderProductStatus = OrderProductStatus.Paid;
            AddEvent(new OrderProductStateChanged(orderProductBeforeChange, this));
        }

        public void OrderProductReleased()
        {
            if (OrderProductStatus != OrderProductStatus.Paid)
            {
                throw new CannotChangeOrderProductStateException(Id, OrderProductStatus, OrderProductStatus.Released);
            }

            var orderProductBeforeChange = new OrderProduct(Id, Quantity, Cost, OrderId, ProductId, OrderProductStatus);
            OrderProductStatus = OrderProductStatus.Released;
            AddEvent(new OrderProductStateChanged(orderProductBeforeChange, this));
        }

        public void AddQuantity(int quantity)
        {
            Quantity += quantity;
        }

        public void DeleteQuantity(int quantity)
        {
            Quantity -= quantity;
        }

        public bool Equals(OrderProduct other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || ProductId.Equals(other.ProductId); // dodane dla hashsetu sprawdzanie po ProductId
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((OrderProduct)obj); // dodane dla hashsetu sprawdzanie po ProductId
        }

        public override int GetHashCode()
        {
            return ProductId.GetHashCode(); // dodane dla hashsetu sprawdzanie po ProductId
        }

        private static void ValidCost(Guid id, decimal cost)
        {
            if (cost < 0)
            {
                throw new InvalidOrderProductCostException(id, cost);
            }
        }

        private static void ValidQuantity(Guid id, int quantity)
        {
            if (quantity <= 0)
            {
                throw new InvalidOrderProductQuantityException(id, quantity);
            }
        }
    }
}

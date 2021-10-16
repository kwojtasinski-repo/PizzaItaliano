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
        public OrderProductStatus OrderProductStatus { get; private set; }

        public OrderProduct(Guid id, int quantity, Guid orderId, Guid productId, OrderProductStatus orderProductStatus)
        {
            Id = id;
            Quantity = quantity;
            OrderId = orderId;
            ProductId = productId;
            OrderProductStatus = orderProductStatus;
        }

        public static OrderProduct Create(Guid id, int quantity, Guid orderId, Guid productId)
        {
            var orderProduct = new OrderProduct(id, quantity, orderId, productId, OrderProductStatus.New);
            return orderProduct;
        }

        public void OrderProductPaid()
        {
            if (OrderProductStatus != OrderProductStatus.New)
            {
                throw new CannotChangeOrderProductStateException(Id, OrderProductStatus, OrderProductStatus.Paid);
            }

            OrderProductStatus = OrderProductStatus.Paid;
        }

        public void OrderProductReleased()
        {
            if (OrderProductStatus != OrderProductStatus.Paid)
            {
                throw new CannotChangeOrderProductStateException(Id, OrderProductStatus, OrderProductStatus.Released);
            }

            OrderProductStatus = OrderProductStatus.Released;
            AddEvent(new OrderProductReleased(this));
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
    }
}

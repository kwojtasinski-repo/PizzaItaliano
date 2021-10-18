﻿using PizzaItaliano.Services.Orders.Core.Events;
using PizzaItaliano.Services.Orders.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Entities
{
    public class Order : AggregateRoot
    {
        private ISet<OrderProduct> _orderProducts = new HashSet<OrderProduct>();
        public string OrderNumber { get; private set; }
        public decimal Cost { get; private set; }
        public bool Paid => OrderStatus != OrderStatus.New;
        public OrderStatus OrderStatus { get; private set; }
        public DateTime OrderDate { get; private set; }
        public DateTime? ReleaseDate { get; private set; }
        public bool HasProducts => _orderProducts.Any();

        public Order(Guid id, string orderNumber, decimal cost, OrderStatus orderStatus, DateTime orderDate, DateTime? releaseDate, IEnumerable<OrderProduct> orderProducts = null, int version = 0)
        {
            Id = id;
            OrderNumber = orderNumber;
            Cost = cost;
            OrderStatus = orderStatus;
            OrderDate = orderDate;
            ReleaseDate = releaseDate;
            OrderProducts = orderProducts ?? Enumerable.Empty<OrderProduct>();
            Version = version;
        }

        public IEnumerable<OrderProduct> OrderProducts
        {
            get => _orderProducts;
            private set => _orderProducts = new HashSet<OrderProduct>(value);
        }

        public static Order Create(Guid id, string orderNumber, decimal cost)
        {
            var order = new Order(id, orderNumber, cost, OrderStatus.New, DateTime.Now, null);
            order.AddEvent(new OrderCreated(order));
            return order;
        }

        public void AddOrderProduct(OrderProduct orderProduct)
        {
            if (!_orderProducts.Add(orderProduct))
            {
                throw new OrderProductAlreadyAddedToOrderException(Id, orderProduct.ProductId);
            }

            AddEvent(new OrderProductCreated(this, orderProduct));
        }

        public void DeleteOrderProduct(Guid orderProductId)
        {
            var orderProduct = _orderProducts.SingleOrDefault(op => op.Id == orderProductId);
            if (orderProduct is null)
            {
                throw new OrderProductNotFoundException(Id, orderProductId);
            }

            AddEvent(new OrderProductRemoved(this, orderProduct));
        }

        public void OrderPaid()
        {
            if (OrderStatus != OrderStatus.New)
            {
                throw new CannotChangeOrderStateException(Id, OrderStatus, OrderStatus.Paid);
            }

            var orderBeforeChange = new Order(Id, OrderNumber, Cost, OrderStatus, OrderDate, ReleaseDate, OrderProducts, Version);

            OrderStatus = OrderStatus.Paid;
            if (HasProducts)
            {
                foreach(var orderProduct in _orderProducts)
                {
                    orderProduct.OrderProductPaid();
                }    
            }
            AddEvent(new OrderStateChanged(orderBeforeChange, this));
        }
        
        public void OrderReleased()
        {
            if (OrderStatus != OrderStatus.Paid || !HasProducts)
            {
                throw new CannotChangeOrderStateException(Id, OrderStatus, OrderStatus.Released);
            }

            var orderProductsReleased = _orderProducts.All(op => op.OrderProductStatus == OrderProductStatus.Released);
            if (!orderProductsReleased)
            {
                throw new CannotChangeOrderStateException(Id, OrderStatus, OrderStatus.Released);
            }

            var orderBeforeChange = new Order(Id, OrderNumber, Cost, OrderStatus, OrderDate, ReleaseDate, OrderProducts, Version);
            OrderStatus = OrderStatus.Released;
            AddEvent(new OrderStateChanged(orderBeforeChange, this));
        }

        public void UpdateCost(decimal cost)
        {
            if (OrderStatus != OrderStatus.New)
            {
                throw new CannotUpdateOrderCostException(Id, cost, OrderStatus);
            }

            Cost += cost;
        }
    }
}
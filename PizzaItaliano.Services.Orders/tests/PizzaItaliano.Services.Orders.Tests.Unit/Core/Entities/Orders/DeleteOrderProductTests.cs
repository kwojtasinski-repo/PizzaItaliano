using PizzaItaliano.Services.Orders.Core.Exceptions;
using PizzaItaliano.Services.Orders.Core.Entities;
using PizzaItaliano.Services.Orders.Core.Events;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.Unit.Core.Entities.Orders
{
    public class DeleteOrderProductTests
    {
        [Fact]
        public void given_valid_parameters_order_should_be_removed()
        {
            // Arrange
            var orderId = new AggregateId();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var number = "ORD/2021/10/31/1";
            var cost = new decimal(12.12);
            var quantity = 1;
            var quantityToDelete = 1;
            var order = new Order(orderId, number, decimal.Zero, OrderStatus.New, DateTime.Now, null);
            var orderProduct = new OrderProduct(orderProductId, quantity, cost, orderId, productId, OrderProductStatus.New);
            order.AddOrderProduct(orderProduct);
            order.ClearEvents();
            var orderProduct2 = new OrderProduct(orderProductId, quantity, cost, orderId, productId, OrderProductStatus.New);

            // Act
            order.DeleteOrderProduct(orderProduct2, quantityToDelete);

            // Assert
            order.Cost.ShouldBe(decimal.Zero);
            order.HasProducts.ShouldBe(false);
            order.OrderProducts.Count().ShouldBe(0);
            var @event = order.Events.Single();
            @event.ShouldBeOfType<OrderProductRemoved>();
        }

        [Fact]
        public void given_invalid_product_id_should_throw_an_exception()
        {
            // Arrange
            var orderId = new AggregateId();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var number = "ORD/2021/10/31/1";
            var cost = new decimal(12.12);
            var quantity = 1;
            var quantityToDelete = 1;
            var order = new Order(orderId, number, decimal.Zero, OrderStatus.New, DateTime.Now, null);
            var orderProduct = new OrderProduct(orderProductId, quantity, cost, orderId, productId, OrderProductStatus.New);

            // Act
            var exception = Record.Exception(() => order.DeleteOrderProduct(orderProduct, quantityToDelete));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<OrderProductNotFoundException>();
        }

        [Fact]
        public void given_invalid_quantity_should_throw_an_exception()
        {
            // Arrange
            var orderId = new AggregateId();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var number = "ORD/2021/10/31/1";
            var cost = new decimal(12.12);
            var quantity = 1;
            var quantityToDelete = -3;
            var order = new Order(orderId, number, decimal.Zero, OrderStatus.New, DateTime.Now, null);
            var orderProduct = new OrderProduct(orderProductId, quantity, cost, orderId, productId, OrderProductStatus.New);
            order.AddOrderProduct(orderProduct);
            var orderProduct2 = new OrderProduct(orderProductId, quantity, cost, orderId, productId, OrderProductStatus.New);

            // Act
            var exception = Record.Exception(() => order.DeleteOrderProduct(orderProduct2, quantityToDelete));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotDeleteOrderProductException>();
        }

        [Fact]
        public void given_too_many_quantity_should_throw_an_exception()
        {
            // Arrange
            var orderId = new AggregateId();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var number = "ORD/2021/10/31/1";
            var cost = new decimal(12.12);
            var quantity = 1;
            var quantityToDelete = 5;
            var order = new Order(orderId, number, decimal.Zero, OrderStatus.New, DateTime.Now, null);
            var orderProduct = new OrderProduct(orderProductId, quantity, cost, orderId, productId, OrderProductStatus.New);
            order.AddOrderProduct(orderProduct);
            var orderProduct2 = new OrderProduct(orderProductId, quantity, cost, orderId, productId, OrderProductStatus.New);

            // Act
            var exception = Record.Exception(() => order.DeleteOrderProduct(orderProduct2, quantityToDelete));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotDeleteOrderProductException>();
        }
    }
}

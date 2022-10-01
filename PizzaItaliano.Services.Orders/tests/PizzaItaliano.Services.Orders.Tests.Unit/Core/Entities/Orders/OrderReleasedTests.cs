using PizzaItaliano.Services.Orders.Core.Entities;
using PizzaItaliano.Services.Orders.Core.Events;
using PizzaItaliano.Services.Orders.Core.Exceptions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.Unit.Core.Entities.Orders
{
    public class OrderReleasedTests
    {
        [Fact]
        public void given_valid_parameters_order_should_mark_as_released()
        {
            // Arrange
            var orderId = new AggregateId();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var number = "ORD/2021/10/31/1";
            var cost = new decimal(12.12);
            var quantity = 1;
            var status = OrderStatus.Paid;
            var statusExpected = OrderStatus.Released;
            var currentTime = DateTime.Now.TimeOfDay;
            var productName = "Product #1";
            var userId = Guid.NewGuid();
            var order = new Order(orderId, number, decimal.Zero, status, DateTime.Now, null, userId);
            var orderProduct = new OrderProduct(orderProductId, quantity, cost, orderId, productId, productName, OrderProductStatus.Released);
            order.AddOrderProduct(orderProduct);
            order.ClearEvents();

            // Act
            order.OrderReleased();

            // Assert
            order.OrderStatus.ShouldBe(statusExpected);
            order.ReleaseDate.Value.TimeOfDay.ShouldBeGreaterThan(currentTime);
            var @event = order.Events.Single();
            @event.ShouldBeOfType<OrderStateChanged>();
        }

        [Theory]
        [InlineData(OrderStatus.New)]
        [InlineData(OrderStatus.Ready)]
        [InlineData(OrderStatus.Released)]
        public void given_invalid_status_should_throw_an_exception(OrderStatus status)
        {
            // Arrange
            var orderId = new AggregateId();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var number = "ORD/2021/10/31/1";
            var cost = new decimal(12.12);
            var userId = Guid.NewGuid();
            var order = new Order(orderId, number, decimal.Zero, status, DateTime.Now, null, userId);

            // Act
            var exception = Record.Exception(() => order.OrderReleased());

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotChangeOrderStateException>();
        }
    }
}

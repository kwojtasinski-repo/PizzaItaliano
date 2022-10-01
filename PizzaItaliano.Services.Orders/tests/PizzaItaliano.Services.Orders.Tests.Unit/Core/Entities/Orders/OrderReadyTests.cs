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
    public class OrderReadyTests
    {
        [Theory]
        [InlineData(OrderStatus.New)]
        [InlineData(OrderStatus.Paid)]
        public void given_valid_parameters_order_should_mark_as_ready(OrderStatus status)
        {
            // Arrange
            var orderId = new AggregateId();
            var number = "ORD/2021/10/31/1";
            var statusExpected = OrderStatus.Ready;
            var userId = Guid.NewGuid();
            var order = new Order(orderId, number, decimal.Zero, status, DateTime.Now, null, userId);

            // Act
            order.OrderReady();

            // Assert
            order.OrderStatus.ShouldBe(statusExpected);
            var @event = order.Events.Single();
            @event.ShouldBeOfType<OrderStateChanged>();
        }

        [Theory]
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
            var exception = Record.Exception(() => order.OrderReady());

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotChangeOrderStateException>();
        }
    }
}

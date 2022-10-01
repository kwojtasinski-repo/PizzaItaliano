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

namespace PizzaItaliano.Services.Orders.Tests.Unit.Core.Entities.OrderProducts
{
    public class PaidOrderProductTests
    {
        [Theory]
        [InlineData(OrderProductStatus.New)]
        [InlineData(OrderProductStatus.Released)]
        public void given_valid_parameters_order_product_should_be_mark_as_paid(OrderProductStatus status)
        {
            // Arrange
            var id = new AggregateId();
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var cost = new decimal(12.12);
            var quantity = 1;
            var statusExpected = OrderProductStatus.Paid;
            var productName = "Product #1";
            var orderProduct = new OrderProduct(id, quantity, cost, orderId, productId, productName, status);

            // Act
            orderProduct.OrderProductPaid();

            // Assert
            orderProduct.OrderProductStatus.ShouldNotBe(status);
            orderProduct.OrderProductStatus.ShouldBe(statusExpected);
            var @event = orderProduct.Events.Single();
            @event.ShouldBeOfType<OrderProductStateChanged>();
        }

        [Theory]
        [InlineData(OrderProductStatus.Paid)]
        public void given_invalid_order_product_status_should_throw_exception(OrderProductStatus status)
        {
            // Arrange
            var id = new AggregateId();
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var cost = new decimal(12.12);
            var quantity = 1;
            var productName = "Product #1";
            var orderProduct = new OrderProduct(id, quantity, cost, orderId, productId, productName, status);

            // Act
            var exception = Record.Exception(() => orderProduct.OrderProductPaid());

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotChangeOrderProductStateException>();
        }        
    }
}

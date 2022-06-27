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
    public class AddOrderProductTests
    {
        [Fact]
        public void given_valid_parameters_order_should_be_created()
        {
            // Arrange
            var orderId = new AggregateId();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var number = "ORD/2021/10/31/1";
            var cost = new decimal(12.12);
            var quantity = 1;
            var order = new Order(orderId, number, decimal.Zero, OrderStatus.New, DateTime.Now, null);
            var productName = "Product #1";
            var orderProduct = new OrderProduct(orderProductId, quantity, cost, orderId, productId, productName, OrderProductStatus.New);

            // Act
            order.AddOrderProduct(orderProduct);

            // Assert
            order.Cost.ShouldBe(cost);
            order.HasProducts.ShouldBe(true);
            order.OrderProducts.Count().ShouldBe(1);
            var @event = order.Events.Single();
            @event.ShouldBeOfType<OrderProductAdd>();
        }
    }
}

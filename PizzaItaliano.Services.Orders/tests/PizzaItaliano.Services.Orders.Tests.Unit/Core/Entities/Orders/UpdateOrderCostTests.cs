using PizzaItaliano.Services.Orders.Core.Entities;
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
    public class UpdateOrderCostTests
    {
        [Fact]
        public void given_valid_parameters_order_cost_should_be_updated()
        {
            // Arrange
            var orderId = new AggregateId();
            var number = "ORD/2021/10/31/1";
            var status = OrderStatus.New;
            var cost = decimal.One;
            var userId = Guid.NewGuid();
            var order = new Order(orderId, number, decimal.Zero, status, DateTime.Now, null, userId);

            // Act
            order.UpdateCost(cost);

            // Assert
            order.Cost.ShouldBe(cost);
        }

        [Fact]
        public void given_invalid_status_should_throw_an_exception()
        {
            // Arrange
            var orderId = new AggregateId();
            var number = "ORD/2021/10/31/1";
            var status = OrderStatus.Ready;
            var cost = decimal.One;
            var userId = Guid.NewGuid();
            var order = new Order(orderId, number, decimal.Zero, status, DateTime.Now, null, userId);

            // Act
            var exception = Record.Exception(() => order.UpdateCost(cost));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotUpdateOrderCostException>();
        }
    }
}

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
    public class CreateOrderTests
    {
        private Order Act(AggregateId id, string number, decimal cost) => Order.Create(id, number, cost);

        [Fact]
        public void given_valid_parameters_order_should_be_created()
        {
            // Arrange
            var id = new AggregateId();
            var number = "ORD/2021/10/31/1";
            var cost = new decimal(12.12);

            // Act
            var order = Act(id, number, cost);

            // Assert
            order.Id.ShouldBe(id);
            order.OrderNumber.ShouldBe(number);
            order.Cost.ShouldBe(cost);
            var @event = order.Events.Single();
            @event.ShouldBeOfType<OrderCreated>();
        }
    }
}

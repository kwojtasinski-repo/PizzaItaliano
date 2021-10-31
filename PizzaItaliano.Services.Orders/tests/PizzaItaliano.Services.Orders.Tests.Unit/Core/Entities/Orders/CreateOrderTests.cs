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
        public void given_valid_parameters_product_should_be_created()
        {
            // Arrange
            var id = new AggregateId();
            var number = "ORD/2021/10/31/1";
            var cost = new decimal(12.12);

            // Act
            var product = Act(id, number, cost);

            // Assert
            product.Id.ShouldBe(id);
            product.OrderNumber.ShouldBe(number);
            product.Cost.ShouldBe(cost);
            var @event = product.Events.Single();
            @event.ShouldBeOfType<OrderCreated>();
        }
    }
}

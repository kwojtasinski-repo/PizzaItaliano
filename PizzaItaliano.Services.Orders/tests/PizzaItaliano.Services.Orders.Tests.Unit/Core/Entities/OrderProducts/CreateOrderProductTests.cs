using PizzaItaliano.Services.Orders.Core.Entities;
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
    public class CreateOrderProductTests
    {
        private OrderProduct Act(AggregateId id, int quantity, decimal cost, Guid orderId, Guid productId) => OrderProduct.Create(id, quantity, cost, orderId, productId);

        [Fact]
        public void given_valid_parameters_order_product_should_be_created()
        {
            // Arrange
            var id = new AggregateId();
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var cost = new decimal(12.12);
            var quantity = 1;

            // Act
            var orderProduct = Act(id, quantity, cost, orderId, productId);

            // Assert
            orderProduct.Id.ShouldBe(id);
            orderProduct.Quantity.ShouldBe(quantity);
            orderProduct.Cost.ShouldBe(cost);
            orderProduct.OrderId.ShouldBe(orderId);
            orderProduct.ProductId.ShouldBe(productId);
        }

        [Fact]
        public void given_invalid_quantity_should_throw_exception()
        {
            // Arrange
            var id = new AggregateId();
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var quantity = -1;
            var cost = new decimal(12.12);

            // Act
            var exception = Record.Exception(() => Act(id, quantity, cost, orderId, productId));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidOrderProductQuantityException>();
        }

        [Fact]
        public void given_invalid_cost_should_throw_exception()
        {
            // Arrange
            var id = new AggregateId();
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var quantity = -1;
            var cost = new decimal(-12.12);

            // Act
            var exception = Record.Exception(() => Act(id, quantity, cost, orderId, productId));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidOrderProductCostException>();
        }
    }
}

using PizzaItaliano.Services.Orders.Core.Entities;
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
        public void given_valid_parameters_product_should_be_created()
        {
            // Arrange
            var id = new AggregateId();
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var cost = new decimal(12.12);
            var quantity = 1;

            // Act
            var product = Act(id, quantity, cost, orderId, productId);

            // Assert
            product.Id.ShouldBe(id);
            product.Quantity.ShouldBe(quantity);
            product.Cost.ShouldBe(cost);
            product.OrderId.ShouldBe(orderId);
            product.ProductId.ShouldBe(productId);
        }
    }
}

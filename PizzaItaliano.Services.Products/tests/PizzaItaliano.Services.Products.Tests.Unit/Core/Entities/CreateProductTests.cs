using PizzaItaliano.Services.Products.Core.Entities;
using PizzaItaliano.Services.Products.Core.Events;
using PizzaItaliano.Services.Products.Core.Exceptions;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace PizzaItaliano.Services.Products.Tests.Unit.Core.Entities
{
    public class CreateProductTests
    {
        private Product Act(AggregateId id, string name, decimal cost) => Product.Create(id, name, cost);

        [Fact]
        public void given_valid_parameters_product_should_be_created()
        {
            // Arrange
            var id = new AggregateId();
            var name = "test";
            var cost = new decimal(12.12);
            var status = ProductStatus.New;

            // Act
            var product = Act(id, name, cost);

            // Assert
            product.Id.ShouldBe(id);
            product.Name.ShouldBe(name);
            product.Cost.ShouldBe(cost);
            product.Status.ShouldBe(status);
            var @event = product.Events.Single();
            @event.ShouldBeOfType<ProductAdded>();
        }

        [Fact]
        public void given_invalid_name_should_throw_exception()
        {
            // Arrange
            var id = new AggregateId();

            // Act
            var exception = Record.Exception(() => Act(id, "", 0));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ProductNameCannotBeEmptyException>();
        }

        [Fact]
        public void given_invalid_cost_should_throw_exception()
        {
            // Arrange
            var id = new AggregateId();
            var name = "123";
            var cost = -10;

            // Act
            var exception = Record.Exception(() => Act(id, name, cost));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidProductCostException>();
        }
    }
}

using PizzaItaliano.Services.Products.Core.Events;
using PizzaItaliano.Services.Products.Core.Entities;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PizzaItaliano.Services.Products.Core.Exceptions;

namespace PizzaItaliano.Services.Products.Tests.Unit.Core.Entities
{
    public class ModifiedProdcutTests
    {
        [Fact]
        public void given_valid_name_should_modified_product()
        {
            // Arrange
            var productToModified = new Product(Guid.NewGuid(), "T1", new decimal(123.123), ProductStatus.New);
            var product = new Product(Guid.NewGuid(), "T1", new decimal(123.123), ProductStatus.New);
            var name = "T2";

            // Act
            productToModified.Modified(name);

            // Assert
            productToModified.Name.ShouldNotBe(product.Name);
            productToModified.Name.ShouldBe(name);
            productToModified.Cost.ShouldBe(product.Cost);
            var @event = productToModified.Events.Single();
            @event.ShouldBeOfType<ProductModified>();
        }

        [Fact]
        public void given_invalid_name_should_throw_an_exception()
        {
            // Arrange
            var productToModified = new Product(Guid.NewGuid(), "T1", new decimal(123.123), ProductStatus.New);
            var name = "";

            // Act
            var exception = Record.Exception(() => productToModified.Modified(name));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ProductNameCannotBeEmptyException>();
        }

        [Fact]
        public void given_valid_cost_should_modified_product()
        {
            // Arrange
            var productToModified = new Product(Guid.NewGuid(), "T1", new decimal(123.123), ProductStatus.New);
            var product = new Product(Guid.NewGuid(), "T1", new decimal(123.123), ProductStatus.New);
            var cost = new decimal(124.42);

            // Act
            productToModified.Modified(cost);

            // Assert
            productToModified.Cost.ShouldNotBe(product.Cost);
            productToModified.Cost.ShouldBe(cost);
            productToModified.Name.ShouldBe(product.Name);
            var @event = productToModified.Events.Single();
            @event.ShouldBeOfType<ProductModified>();
        }

        [Fact]
        public void given_invalid_cost_should_throw_an_exception()
        {
            // Arrange
            var productToModified = new Product(Guid.NewGuid(), "T1", new decimal(123.123), ProductStatus.New);
            var cost = new decimal(-123);

            // Act
            var exception = Record.Exception(() => productToModified.Modified(cost));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidProductCostException>();
        }

        [Fact]
        public void given_valid_name_and_cost_should_modified_product()
        {
            // Arrange
            var productToModified = new Product(Guid.NewGuid(), "T1", new decimal(123.123), ProductStatus.New);
            var product = new Product(Guid.NewGuid(), "T1", new decimal(123.123), ProductStatus.New);
            var name = "abc";
            var cost = new decimal(999.99);

            // Act
            productToModified.Modified(name, cost);

            // Assert
            productToModified.Name.ShouldNotBe(product.Name);
            productToModified.Name.ShouldBe(name);
            productToModified.Cost.ShouldBe(cost);
            productToModified.Cost.ShouldNotBe(product.Cost);
            var @event = productToModified.Events.Single();
            @event.ShouldBeOfType<ProductModified>();
        }

        [Fact]
        public void given_invalid_name_and_valid_cost_should_throw_an_exception()
        {
            // Arrange
            var productToModified = new Product(Guid.NewGuid(), "T1", new decimal(123.123), ProductStatus.New);
            var name = "";
            var cost = new decimal(999.99);

            // Act
            var exception = Record.Exception(() => productToModified.Modified(name, cost));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ProductNameCannotBeEmptyException>();
        }

        [Fact]
        public void given_valid_name_and_invalid_cost_should_throw_an_exception()
        {
            // Arrange
            var productToModified = new Product(Guid.NewGuid(), "T1", new decimal(123.123), ProductStatus.New);
            var name = "abc";
            var cost = new decimal(-999.99);

            // Act
            var exception = Record.Exception(() => productToModified.Modified(name, cost));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidProductCostException>();
        }
    }
}

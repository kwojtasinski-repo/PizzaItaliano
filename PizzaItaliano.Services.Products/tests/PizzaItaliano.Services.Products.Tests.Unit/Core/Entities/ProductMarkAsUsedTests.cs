using PizzaItaliano.Services.Products.Core.Entities;
using PizzaItaliano.Services.Products.Core.Exceptions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Products.Tests.Unit.Core.Entities
{
    public class ProductMarkAsUsedTests
    {
        [Fact]
        public void given_valid_product_should_mark_as_used()
        {
            // Arrange
            var productToModified = new Product(Guid.NewGuid(), "T1", new decimal(123.123), ProductStatus.New);
            var product = new Product(Guid.NewGuid(), "T1", new decimal(123.123), ProductStatus.New);

            // Act
            productToModified.MarkAsUsed();

            // Assert
            productToModified.Status.ShouldNotBe(product.Status);
            productToModified.Status.ShouldBe(ProductStatus.Used);
        }

        [Fact]
        public void given_invalid_product_shouldnt_mark_as_used()
        {
            // Arrange
            var productToModified = new Product(Guid.NewGuid(), "T1", new decimal(123.123), ProductStatus.Used);

            // Act
            var exception = Record.Exception(() => productToModified.MarkAsUsed());

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotChangeProductStatusException>();
        }
    }
}

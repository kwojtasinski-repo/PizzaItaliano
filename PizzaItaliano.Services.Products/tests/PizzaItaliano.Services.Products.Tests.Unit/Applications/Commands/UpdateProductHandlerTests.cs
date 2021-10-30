using NSubstitute;
using PizzaItaliano.Services.Products.Application.Commands;
using PizzaItaliano.Services.Products.Application.Commands.Handlers;
using PizzaItaliano.Services.Products.Application.Exceptions;
using PizzaItaliano.Services.Products.Application.Services;
using PizzaItaliano.Services.Products.Core;
using PizzaItaliano.Services.Products.Core.Entities;
using PizzaItaliano.Services.Products.Core.Repositories;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Products.Tests.Unit.Applications.Commands
{
    public class UpdateProductHandlerTests
    {
        private Task Act(UpdateProduct command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_valid_id_product_should_be_updated()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "productModified";
            var cost = new decimal(200.12);
            var command = new UpdateProduct(id, name, cost);
            var product = new Product(id, "product", new decimal(20.12), ProductStatus.New);
            _productRepository.GetAsync(id).Returns(product);

            // Act
            await Act(command);

            // Assert
            await _productRepository.Received(1).GetAsync(id);
            await _productRepository.Received(1).UpdateAsync(Arg.Any<Product>());
            await _eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
        }

        [Fact]
        public async Task given_invalid_parameters_should_throw_an_exception()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new UpdateProduct(id, "", null);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidUpdateProductException>();
        }

        [Fact]
        public async Task given_invalid_cost_should_throw_an_exception()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new UpdateProduct(id, "", new decimal(-20.12));

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidProductCostException>();
        }

        [Fact]
        public async Task given_invalid_id_should_throw_an_exception()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new UpdateProduct(id, "", new decimal(20.12));

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ProductNotFoundException>();
        }

        #region Arrange

        private readonly UpdateProductHandler _handler;
        private readonly IProductRepository _productRepository;
        private readonly IEventProcessor _eventProcessor;

        public UpdateProductHandlerTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _eventProcessor = Substitute.For<IEventProcessor>();
            _handler = new UpdateProductHandler(_productRepository, _eventProcessor);
        }

        #endregion
    }
}

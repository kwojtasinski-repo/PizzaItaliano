using NSubstitute;
using PizzaItaliano.Services.Products.Application.Commands;
using PizzaItaliano.Services.Products.Application.Commands.Handlers;
using PizzaItaliano.Services.Products.Application.Exceptions;
using PizzaItaliano.Services.Products.Application.Services;
using PizzaItaliano.Services.Products.Core;
using PizzaItaliano.Services.Products.Core.Entities;
using PizzaItaliano.Services.Products.Core.Exceptions;
using PizzaItaliano.Services.Products.Core.Repositories;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using InvalidProductCostException = PizzaItaliano.Services.Products.Core.Exceptions.InvalidProductCostException;

namespace PizzaItaliano.Services.Products.Tests.Unit.Applications.Commands
{
    public class AddProductHandlerTests
    {
        private Task Act(AddProduct command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_valid_parameters_product_should_be_created()
        {
            // Arrange
            var name = "product";
            var cost = new decimal(12.14);
            var command = new AddProduct(Guid.NewGuid(), name, cost);

            // Act
            await Act(command);

            // Assert
            await _productRepository.Received(1).AddAsync(Arg.Any<Product>());
            await _eventProcessor.Received(1).ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>());
        }

        [Fact]
        public async Task given_invalid_name_should_throw_an_exception()
        {
            // Arrange
            var name = "";
            var cost = new decimal(12.14);
            var command = new AddProduct(Guid.NewGuid(), name, cost);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ProductNameCannotBeEmptyException>();
        }

        [Fact]
        public async Task given_invalid_cost_should_throw_an_exception()
        {
            // Arrange
            var name = "product";
            var cost = new decimal(-12.14);
            var command = new AddProduct(Guid.NewGuid(), name, cost);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidProductCostException>();
        }

        [Fact]
        public async Task given_existed_product_should_throw_an_exception()
        {
            // Arrange
            var name = "product";
            var cost = new decimal(12.14);
            var command = new AddProduct(Guid.NewGuid(), name, cost);
            _productRepository.ExistsAsync(command.ProductId).Returns(true);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ProductAlreadyExistsException>();
        }

        #region Arrange

        private readonly AddProductHandler _handler;
        private readonly IProductRepository _productRepository;
        private readonly IEventProcessor _eventProcessor;

        public AddProductHandlerTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _eventProcessor = Substitute.For<IEventProcessor>();
            _handler = new AddProductHandler(_productRepository, _eventProcessor);
        }

        #endregion
    }
}

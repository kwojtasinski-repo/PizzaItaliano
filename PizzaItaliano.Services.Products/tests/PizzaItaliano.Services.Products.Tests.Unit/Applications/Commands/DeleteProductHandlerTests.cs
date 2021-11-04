using Convey.CQRS.Events;
using NSubstitute;
using PizzaItaliano.Services.Products.Application.Commands;
using PizzaItaliano.Services.Products.Application.Commands.Handlers;
using PizzaItaliano.Services.Products.Application.Exceptions;
using PizzaItaliano.Services.Products.Application.Services;
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
    public class DeleteProductHandlerTests
    {
        private Task Act(DeleteProduct command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_valid_id_product_should_be_deleted()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteProduct(id);
            var product = new Product(id, "product", new decimal(20.12), ProductStatus.New);
            _productRepository.GetAsync(id).Returns(product);

            // Act
            await Act(command);

            // Assert
            await _productRepository.Received(1).DeleteAsync(id);
            await _messageBroker.Received(1).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_invalid_id_should_throw_an_exception()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteProduct(id);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ProductNotFoundException>();
        }

        [Fact]
        public async Task given_used_product_id_should_throw_an_exception()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteProduct(id);
            var product = new Product(id, "product", new decimal(20.12), ProductStatus.Used);
            _productRepository.GetAsync(id).Returns(product);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotDeleteProductException>();
        }

        #region Arrange

        private readonly DeleteProductHandler _handler;
        private readonly IProductRepository _productRepository;
        private readonly IMessageBroker _messageBroker;

        public DeleteProductHandlerTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _messageBroker = Substitute.For<IMessageBroker>();
            _handler = new DeleteProductHandler(_productRepository, _messageBroker);
        }

        #endregion
    }
}

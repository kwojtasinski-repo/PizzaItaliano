using NSubstitute;
using PizzaItaliano.Services.Products.Application.Events.External;
using PizzaItaliano.Services.Products.Application.Events.External.Handlers;
using PizzaItaliano.Services.Products.Core.Entities;
using PizzaItaliano.Services.Products.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Products.Tests.Unit.Applications.Events
{
    public class OrderProductAddedHandlerTests
    {
        private Task Act(OrderProductAdded command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_valid_id_product_should_mark_product_as_used()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var name = "product";
            var cost = new decimal(200.12);
            var command = new OrderProductAdded(orderId, orderProductId, productId);
            var product = new Product(productId, name, cost, ProductStatus.New);
            _productRepository.GetAsync(productId).Returns(product);

            // Act
            await Act(command);

            // Assert
            await _productRepository.Received(1).GetAsync(productId);
            await _productRepository.Received(1).UpdateAsync(Arg.Any<Product>());
        }

        [Fact]
        public async Task given_invalid_parameters_shouldnt_mark_product_as_used()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var command = new OrderProductAdded(productId, orderId, orderProductId);

            // Act
            await Act(command);

            // Assert
            await _productRepository.Received(0).GetAsync(productId);
            await _productRepository.Received(0).UpdateAsync(Arg.Any<Product>());
        }

        [Fact]
        public async Task given_used_product_id_shouldnt_mark_product_as_used()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var command = new OrderProductAdded(productId, orderId, orderProductId);

            // Act
            await Act(command);

            // Assert
            await _productRepository.Received(0).GetAsync(productId);
            await _productRepository.Received(0).UpdateAsync(Arg.Any<Product>());
        }

        #region Arrange

        private readonly OrderProductAddedHandler _handler;
        private readonly IProductRepository _productRepository;

        public OrderProductAddedHandlerTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _handler = new OrderProductAddedHandler(_productRepository);
        }

        #endregion
    }
}

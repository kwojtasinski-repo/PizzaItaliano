using Convey.CQRS.Events;
using NSubstitute;
using PizzaItaliano.Services.Orders.Application.Commands;
using PizzaItaliano.Services.Orders.Application.Commands.Handlers;
using PizzaItaliano.Services.Orders.Application.DTO;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Application.Services;
using PizzaItaliano.Services.Orders.Application.Services.Clients;
using PizzaItaliano.Services.Orders.Core.Entities;
using PizzaItaliano.Services.Orders.Core.Repositories;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.Unit.Applications.Commands.OrderProducts
{
    public class AddOrderProductHandlerTests
    {
        private Task Act(AddOrderProduct command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_valid_parameters_order_product_should_be_added()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var quantity = 1;
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);
            var order = new Order(orderId, "abc", decimal.Zero, OrderStatus.New, DateTime.Now, null, Guid.NewGuid());
            var product = new ProductDto() { Id = productId, Cost = decimal.One, Name = "product", ProductStatus = ProductStatus.Used };
            _orderRepository.GetWithCollectionAsync(orderId).Returns(order);
            _productServiceClient.GetAsync(productId).Returns(product);

            // Act
            await Act(command);

            // Assert
            await _orderRepository.Received(1).UpdateAsync(Arg.Any<Order>());
            await _messageBroker.Received(1).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_invalid_quantity_should_throw_an_exception()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var quantity = -1;
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotAddOrderProductException>();
        }

        [Fact]
        public async Task given_invalid_order_id_should_throw_an_exception()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var quantity = 1;
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<OrderNotFoundException>();
        }

        [Fact]
        public async Task given_order_with_invalid_status_should_throw_an_exception()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var quantity = 1;
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);
            var order = new Order(orderId, "abc", decimal.Zero, OrderStatus.Ready, DateTime.Now, null,Guid.NewGuid());
            _orderRepository.GetWithCollectionAsync(orderId).Returns(order);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<CannotAddOrderProductException>();
        }

        [Fact]
        public async Task given_invalid_product_id_should_throw_an_exception()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var quantity = 1;
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);
            var order = new Order(orderId, "abc", decimal.Zero, OrderStatus.New, DateTime.Now, null, Guid.NewGuid());
            _orderRepository.GetWithCollectionAsync(orderId).Returns(order);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ProductNotFoundException>();
        }

        #region Arrange

        private readonly AddOrderProductHandler _handler;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductServiceClient _productServiceClient;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public AddOrderProductHandlerTests()
        {
            _orderRepository = Substitute.For<IOrderRepository>();
            _productServiceClient = Substitute.For<IProductServiceClient>();
            _messageBroker = Substitute.For<IMessageBroker>();
            _eventMapper = Substitute.For<IEventMapper>();
            _handler = new AddOrderProductHandler(_orderRepository, _productServiceClient, _messageBroker, _eventMapper);
        }

        #endregion
    }
}

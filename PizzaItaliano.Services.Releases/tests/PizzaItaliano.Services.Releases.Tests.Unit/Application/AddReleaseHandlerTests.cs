using Convey.CQRS.Events;
using NSubstitute;
using PizzaItaliano.Services.Releases.Application.Commands;
using PizzaItaliano.Services.Releases.Application.Commands.Handlers;
using PizzaItaliano.Services.Releases.Application.Exceptions;
using PizzaItaliano.Services.Releases.Application.Services;
using PizzaItaliano.Services.Releases.Core.Entities;
using PizzaItaliano.Services.Releases.Core.Repositories;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Releases.Tests.Unit.Application
{
    public class AddReleaseHandlerTests
    {
        private Task Act(AddRelease command) => _handler.HandleAsync(command);

        [Fact]
        public async Task given_valid_parameters_release_should_be_added()
        {
            // Arrange
            var releaseId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var command = new AddRelease() { OrderId = orderId, OrderProductId = orderProductId, ReleaseId = releaseId };

            // Act
            await Act(command);

            // Assert
            await _releaseRepository.Received(1).AddAsync(Arg.Any<Release>());
            await _messageBroker.Received(1).PublishAsync(Arg.Any<IEnumerable<IEvent>>());
        }

        [Fact]
        public async Task given_existed_release_should_throw_an_exception()
        {
            // Arrange
            var releaseId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var command = new AddRelease() { OrderId = orderId, OrderProductId = orderProductId, ReleaseId = releaseId };
            _releaseRepository.ExistsAsync(releaseId).Returns(true);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ReleaseAlreadyExistsException>();
        }

        #region Arrange

        private readonly AddReleaseHandler _handler;
        private readonly IReleaseRepository _releaseRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public AddReleaseHandlerTests()
        {
            _releaseRepository = Substitute.For<IReleaseRepository>();
            _messageBroker = Substitute.For<IMessageBroker>();
            _eventMapper = Substitute.For<IEventMapper>();
            _handler = new AddReleaseHandler(_releaseRepository, _messageBroker, _eventMapper);
        }

        #endregion
    }
}

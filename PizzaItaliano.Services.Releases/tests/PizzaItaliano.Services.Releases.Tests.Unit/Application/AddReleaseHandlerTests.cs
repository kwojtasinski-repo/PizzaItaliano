﻿using Convey.CQRS.Events;
using NSubstitute;
using PizzaItaliano.Services.Releases.Application;
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

        [Fact]
        public async Task given_empty_user_id_should_throw_an_exception()
        {
            // Arrange
            var releaseId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var command = new AddRelease() { OrderId = orderId, OrderProductId = orderProductId, ReleaseId = releaseId };
            _identityContext.Id.Returns(Guid.Empty);

            // Act
            var exception = await Record.ExceptionAsync(() => Act(command));

            // Assert
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidUserIdException>();
        }

        #region Arrange

        private readonly AddReleaseHandler _handler;
        private readonly IReleaseRepository _releaseRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;
        private readonly IAppContext _appContext;
        private readonly IIdentityContext _identityContext;

        public AddReleaseHandlerTests()
        {
            _releaseRepository = Substitute.For<IReleaseRepository>();
            _messageBroker = Substitute.For<IMessageBroker>();
            _eventMapper = Substitute.For<IEventMapper>();
            _appContext = Substitute.For<IAppContext>();
            _appContext.RequestId.Returns(Guid.NewGuid().ToString("N"));
            _identityContext = Substitute.For<IIdentityContext>();
            _identityContext.Id.Returns(Guid.NewGuid());
            _identityContext.Role.Returns("admin");
            _identityContext.IsAuthenticated.Returns(true);
            _identityContext.IsAdmin.Returns(true);
            _identityContext.Claims.Returns(new Dictionary<string, string>());
            _appContext.Identity.Returns(_identityContext);
            _handler = new AddReleaseHandler(_releaseRepository, _messageBroker, _eventMapper, _appContext);
        }

        #endregion
    }
}

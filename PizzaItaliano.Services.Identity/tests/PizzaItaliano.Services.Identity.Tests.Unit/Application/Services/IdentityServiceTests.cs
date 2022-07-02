using Convey.CQRS.Events;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PizzaItaliano.Services.Identity.Application.Commands;
using PizzaItaliano.Services.Identity.Application.DTO;
using PizzaItaliano.Services.Identity.Application.Services;
using PizzaItaliano.Services.Identity.Application.Services.Identity;
using PizzaItaliano.Services.Identity.Core.Entities;
using PizzaItaliano.Services.Identity.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.Unit.Application.Services
{
    public class IdentityServiceTests
    {
        [Fact]
        public async Task should_sign_in()
        {
            var command = new SignIn("test@test.abc", "PasW0Rd12!");
            var user = CreateUser(command.Email, command.Password, "user");
            _passwordService.IsValid(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            _userRepository.GetAsync(command.Email).Returns(user);
            _jwtProvider.Create(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<IDictionary<string, IEnumerable<string>>>())
                .Returns(new AuthDto { AccessToken = "token", Expires = DateTime.Now.AddHours(1.0).Ticks, RefreshToken = "", Role = user.Role });
            _refreshTokenService.CreateAsync(user.Id).Returns("string");

            await _identityService.SignInAsync(command);

            await _messageBroker.Received(1).PublishAsync(Arg.Any<IEvent[]>());
        }

        [Fact]
        public async Task should_sign_up()
        {
            var command = new SignUp(Guid.NewGuid(), "Email@email.com", "PAs@WAORD121", null, null);
            _passwordService.Hash(command.Password).Returns("Hash1234123!@4");

            await _identityService.SignUpAsync(command);

            await _userRepository.Received(1).AddAsync(Arg.Any<User>());
            await _messageBroker.Received(1).PublishAsync(Arg.Any<IEvent[]>());
        }

        private User CreateUser(string email, string password, string role, IEnumerable<string> permissions = null)
        {
            var user = new User(Guid.NewGuid(), email, password, role, DateTime.UtcNow, permissions);
            return user;
        }

        private readonly IdentityService _identityService;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IMessageBroker _messageBroker;
        private readonly ILogger<IdentityService> _logger;

        public IdentityServiceTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _passwordService = Substitute.For<IPasswordService>();
            _jwtProvider = Substitute.For<IJwtProvider>();
            _refreshTokenService = Substitute.For<IRefreshTokenService>();
            _messageBroker = Substitute.For<IMessageBroker>();
            _logger = Substitute.For<ILogger<IdentityService>>();
            _identityService = new IdentityService(_userRepository, _passwordService, _jwtProvider, 
                _refreshTokenService, _messageBroker, _logger);
        }
    }
}

using NSubstitute;
using PizzaItaliano.Services.Identity.Application.DTO;
using PizzaItaliano.Services.Identity.Application.Services;
using PizzaItaliano.Services.Identity.Application.Services.Identity;
using PizzaItaliano.Services.Identity.Core.Entities;
using PizzaItaliano.Services.Identity.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.Unit.Application.Services
{
    public class RefreshTokenServiceTests
    {
        [Fact]
        public async Task should_revoke()
        {
            var token = "Token123";
            _refreshTokenRepository.GetAsync(token).Returns(new RefreshToken(Guid.NewGuid(), Guid.NewGuid(), token, DateTime.Now));

            await _refreshTokenService.RevokeAsync(token);

            await _refreshTokenRepository.Received(1).UpdateAsync(Arg.Any<RefreshToken>());
        }

        [Fact]
        public async Task should_use_token()
        {
            var token = "Token123";
            var userId = Guid.NewGuid();
            var user = CreateUser("Email@Email.com", "PasswordAb12", "user");
            _refreshTokenRepository.GetAsync(token).Returns(new RefreshToken(Guid.NewGuid(), userId, token, DateTime.Now));
            _userRepository.GetAsync(userId).Returns(user);
            _jwtProvider.Create(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<IDictionary<string, IEnumerable<string>>>())
                .Returns(new AuthDto { AccessToken = "token", Expires = DateTime.Now.AddHours(1.0).Ticks, RefreshToken = "", Role = user.Role });

            await _refreshTokenService.UseAsync(token);
        }

        private User CreateUser(string email, string password, string role, IEnumerable<string> permissions = null)
        {
            var user = new User(Guid.NewGuid(), email, password, role, DateTime.UtcNow, permissions);
            return user;
        }

        private readonly RefreshTokenService _refreshTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IRng _rng;

        public RefreshTokenServiceTests()
        {
            _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _jwtProvider = Substitute.For<IJwtProvider>();
            _rng = Substitute.For<IRng>();
            _refreshTokenService = new RefreshTokenService(_refreshTokenRepository, _userRepository,
                _jwtProvider, _rng);
        }
    }
}

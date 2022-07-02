using Microsoft.Extensions.Logging;
using PizzaItaliano.Services.Identity.Application.Commands;
using PizzaItaliano.Services.Identity.Application.DTO;
using PizzaItaliano.Services.Identity.Application.Events;
using PizzaItaliano.Services.Identity.Application.Exceptions;
using PizzaItaliano.Services.Identity.Core.Entities;
using PizzaItaliano.Services.Identity.Core.Entities.ValueObjects;
using PizzaItaliano.Services.Identity.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Application.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IMessageBroker _messageBroker;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(IUserRepository userRepository, IPasswordService passwordService, IJwtProvider jwtProvider,
            IRefreshTokenService refreshTokenService, IMessageBroker messageBroker, ILogger<IdentityService> logger)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtProvider = jwtProvider;
            _refreshTokenService = refreshTokenService;
            _messageBroker = messageBroker;
            _logger = logger;
        }

        public async Task<UserDto> GetAsync(Guid id)
        {
            var user = await _userRepository.GetAsync(id);
            return user is null ? null : new UserDto(user);
        }

        public async Task<AuthDto> SignInAsync(SignIn command)
        {
            var email = Email.From(command.Email);
            var user = await _userRepository.GetAsync(email.Value);
            var test1 = _passwordService.IsValid(user.Password, command.Password);
            var test2 = user is null;
            if (user is null || !_passwordService.IsValid(user.Password, command.Password))
            {
                _logger.LogError($"Invalid password for user with id: {user.Id.Value}");
                throw new InvalidCredentialsException(command.Email);
            }

            var claims = user.Permissions.Any() ? new Dictionary<string, IEnumerable<string>>
            {
                ["permissions"] = user.Permissions
            } : null;

            var auth = _jwtProvider.Create(user.Id, user.Role, claims: claims);
            auth.RefreshToken = await _refreshTokenService.CreateAsync(user.Id);

            _logger.LogInformation($"User with id: {user.Id} has been authenticated.");
            await _messageBroker.PublishAsync(new SignedIn(user.Id, user.Role));

            return auth;
        }

        public async Task SignUpAsync(SignUp command)
        {
            var email = Email.From(command.Email);
            var user = await _userRepository.GetAsync(email.Value);

            if (user is not null)
            {
                _logger.LogError($"Email already in use: {command.Email}");
                throw new EmailInUseException(command.Email);
            }

            var role = string.IsNullOrWhiteSpace(command.Role) ? "user" : command.Role.ToLowerInvariant();
            var password = Password.From(command.Password);
            var passwordHash = _passwordService.Hash(password.Value);
            user = new User(command.UserId, command.Email, passwordHash, role, DateTime.UtcNow, command.Permissions);
            await _userRepository.AddAsync(user);

            _logger.LogInformation($"Created an account for the user with id: {user.Id}.");
            await _messageBroker.PublishAsync(new SignedUp(user.Id, user.Email.Value, user.Role));
        }
    }
}
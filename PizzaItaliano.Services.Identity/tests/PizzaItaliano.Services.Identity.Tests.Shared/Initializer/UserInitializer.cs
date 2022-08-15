using Microsoft.Extensions.Hosting;
using PizzaItaliano.Services.Identity.Application.Services;
using PizzaItaliano.Services.Identity.Core.Entities;
using PizzaItaliano.Services.Identity.Core.Repositories;
using System.Threading;
using System.Threading.Tasks;
using static PizzaItaliano.Services.Identity.Tests.Shared.Helpers.TestUsers;

namespace PizzaItaliano.Services.Identity.Tests.Shared.Initializer
{
    internal class UserInitializer : IHostedService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public UserInitializer(IPasswordService passwordService, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var users = GetTestUsers();

            foreach(var user in users)
            {
                var userToAdd = new UserDocument(user);
                userToAdd.Password = _passwordService.Hash(userToAdd.Password);
                await _userRepository.AddAsync(MapToUserDocument.Map(userToAdd));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private class MapToUserDocument
        {
            public static User Map(UserDocument userDocument)
            {
                return new User(userDocument.Id, userDocument.Email, userDocument.Password, userDocument.Role, userDocument.CreatedAt, userDocument.Permissions);
            }
        }
    }
}

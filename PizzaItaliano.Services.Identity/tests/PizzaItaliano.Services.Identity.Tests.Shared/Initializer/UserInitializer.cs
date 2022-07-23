using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using PizzaItaliano.Services.Identity.Application.Services;
using PizzaItaliano.Services.Identity.Tests.Shared.Fixtures;
using System;
using System.Threading;
using System.Threading.Tasks;
using static PizzaItaliano.Services.Identity.Tests.Shared.Helpers.TestUsers;

namespace PizzaItaliano.Services.Identity.Tests.Shared.Initializer
{
    internal class UserInitializer : IHostedService
    {
        private readonly MongoDbFixture<UserDocument, Guid> _mongoDbFixture;
        private readonly IPasswordService _passwordService;

        public UserInitializer(IPasswordService passwordService)
        {
            _mongoDbFixture = new MongoDbFixture<UserDocument, Guid>("users");
            _passwordService = passwordService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var users = GetTestUsers();

            foreach(var user in users)
            {
                var userToAdd = new UserDocument(user);
                userToAdd.Password = _passwordService.Hash(userToAdd.Password);
                await _mongoDbFixture.InsertAsync(userToAdd);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

using Microsoft.Extensions.Hosting;
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

        public UserInitializer()
        {
            _mongoDbFixture = new MongoDbFixture<UserDocument, Guid>("users");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var users = GetTestUsers();

            foreach(var user in users)
            {
                await _mongoDbFixture.InsertAsync(user);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

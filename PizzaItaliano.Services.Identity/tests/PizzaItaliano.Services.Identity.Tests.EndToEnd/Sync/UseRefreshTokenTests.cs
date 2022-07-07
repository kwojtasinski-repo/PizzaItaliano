using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Identity.Application.Services;
using PizzaItaliano.Services.Identity.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Identity.Tests.Shared.Factories;
using PizzaItaliano.Services.Identity.Tests.Shared.Fixtures;
using System;
using System.Net.Http;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.EndToEnd.Sync
{
    [Collection("Collection")]
    public class UseRefreshTokenTests
    {

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<UserDocument, Guid> _mongoDbFixture;
        private readonly IIdentityService _identityService;

        public UseRefreshTokenTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<UserDocument, Guid>("refreshTokens");
            _httpClient = factory.CreateClient();
            factory.Server.AllowSynchronousIO = true;
            _identityService = factory.Services.GetRequiredService<IIdentityService>();
        }

        #endregion
    }
}

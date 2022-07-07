using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Identity.Application.Commands;
using PizzaItaliano.Services.Identity.Application.Exceptions;
using PizzaItaliano.Services.Identity.Application.Services;
using PizzaItaliano.Services.Identity.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Identity.Tests.EndToEnd.Helpers;
using PizzaItaliano.Services.Identity.Tests.Shared.Factories;
using PizzaItaliano.Services.Identity.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.EndToEnd.Sync
{
    [Collection("Collection")]
    public class RevokeRefreshTokenTests
    {
        private Task<HttpResponseMessage> Act(RevokeRefreshToken command)
           => _httpClient.PostAsync("refresh-tokens/revoke", TestHelper.GetContent(command));

        [Fact]
        public async Task should_revoke_refresh_token()
        {
            var signUp = new SignUp(Guid.Empty, "emailabc123b83a2@email.com", "PAsW0RDd13!2", "admin", Enumerable.Empty<string>());
            await _identityService.SignUpAsync(signUp);
            var auth = await _identityService.SignInAsync(new SignIn(signUp.Email, signUp.Password));
            var command = new RevokeRefreshToken(auth.RefreshToken);

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var token = await _mongoDbFixture.GetByExpression(e => e.Token == auth.RefreshToken);
            token.ShouldNotBeNull();
            token.RevokedAt.ShouldNotBeNull();
        }

        [Fact]
        public async Task revoke_refresh_token_endpoint_with_empty_refresh_token_should_return_http_status_code_bad_request()
        {
            var command = new RevokeRefreshToken("");

            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new InvalidRefreshTokenException();
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<RefreshTokenDocument, Guid> _mongoDbFixture;
        private readonly IIdentityService _identityService;

        public RevokeRefreshTokenTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<RefreshTokenDocument, Guid>("refreshTokens");
            _httpClient = factory.CreateClient();
            factory.Server.AllowSynchronousIO = true;
            _identityService = factory.Services.GetRequiredService<IIdentityService>();
        }

        #endregion
    }
}

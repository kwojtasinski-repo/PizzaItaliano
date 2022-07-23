using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Identity.Application.Commands;
using PizzaItaliano.Services.Identity.Application.Services;
using PizzaItaliano.Services.Identity.Tests.EndToEnd.Helpers;
using PizzaItaliano.Services.Identity.Tests.Shared.Factories;
using PizzaItaliano.Services.Identity.Tests.Shared.Helpers;
using Shouldly;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.EndToEnd.Sync
{
    [Collection("Collection")]
    public class RevokeAccessTokenTests
    {
        private Task<HttpResponseMessage> Act(RevokeAccessToken command)
           => _httpClient.PostAsync("access-tokens/revoke", TestHelper.GetContent(command));

        [Fact]
        public async Task revoke_endpoint_should_return_no_content()
        {
            var user = TestUsers.GetTestUsers().FirstOrDefault();
            var auth = await _identityService.SignInAsync(new SignIn(user.Email, user.Password));
            var command = new RevokeAccessToken(auth.AccessToken);

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task revoked_token_shouldnt_get_any_authorized_endpoint()
        {
            var user = TestUsers.GetTestUsers().FirstOrDefault();
            var auth = await _identityService.SignInAsync(new SignIn(user.Email, user.Password));
            var command = new RevokeAccessToken(auth.AccessToken);
            await Act(command);

            _httpClient.AddBearerTokenToHeader(auth.AccessToken);
            var response = await _httpClient.GetAsync("me");

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly IIdentityService _identityService;

        public RevokeAccessTokenTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateClient();
            factory.Server.AllowSynchronousIO = true;
            _identityService = factory.Services.GetRequiredService<IIdentityService>();
        }

        #endregion
    }
}

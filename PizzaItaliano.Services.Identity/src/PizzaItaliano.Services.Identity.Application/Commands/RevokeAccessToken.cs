using Convey.CQRS.Commands;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Application.Commands
{
    public class RevokeAccessToken : ICommand
    {
        public string AccessToken { get; }

        public RevokeAccessToken(string accessToken)
        {
            AccessToken = accessToken;
        }
    }

    internal class RevokeAccessTokenHandler : ICommandHandler<RevokeAccessToken>
    {
        public Task HandleAsync(RevokeAccessToken command)
        {
            return Task.CompletedTask;
        }
    }
}
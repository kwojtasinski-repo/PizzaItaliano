using Convey.CQRS.Commands;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Application.Commands
{
    public class RevokeRefreshToken : ICommand
    {
        public string RefreshToken { get; }

        public RevokeRefreshToken(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }

    internal class RevokeRefreshTokenHandler : ICommandHandler<RevokeRefreshToken>
    {
        public Task HandleAsync(RevokeRefreshToken command)
        {
            return Task.CompletedTask;
        }
    }
}
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

    internal class RevokeRefreshTokenHandler : ICommandHandler<SignIn>
    {
        public Task HandleAsync(SignIn command)
        {
            return Task.CompletedTask;
        }
    }
}
using Convey.CQRS.Commands;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Application.Commands
{
    public class UseRefreshToken : ICommand
    {
        public string RefreshToken { get; }

        public UseRefreshToken(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }

    internal class UseRefreshTokenHandler : ICommandHandler<UseRefreshToken>
    {
        public Task HandleAsync(UseRefreshToken command)
        {
            return Task.CompletedTask;
        }
    }
}
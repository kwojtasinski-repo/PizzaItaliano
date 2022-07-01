using Convey.CQRS.Commands;
using PizzaItaliano.Services.Identity.Application.Services;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Application.Commands.Handlers
{
    internal class SignInHandler : ICommandHandler<SignIn>
    {
        private readonly IIdentityService _identityService;

        public SignInHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public Task HandleAsync(SignIn command) => _identityService.SignInAsync(command);
    }
}

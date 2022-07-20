using Convey.CQRS.Commands;
using PizzaItaliano.Services.Identity.Application.Services;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Application.Commands.Handlers
{
    internal sealed class ChangeUserRoleHandler : ICommandHandler<ChangeUserRole>
    {
        private readonly IIdentityService _identityService;

        public ChangeUserRoleHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task HandleAsync(ChangeUserRole command)
        {
            await _identityService.ChangeRoleAsync(command);
        }
    }
}

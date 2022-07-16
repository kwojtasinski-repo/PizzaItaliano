using Convey.CQRS.Commands;
using PizzaItaliano.Services.Identity.Application.Services;
using System;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Application.Commands.Handlers
{
    internal sealed class ChangePasswordHandler : ICommandHandler<ChangePassword>
    {
        private readonly IIdentityService _identityService;

        public ChangePasswordHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task HandleAsync(ChangePassword command)
        {
            await _identityService.ChangePasswordAsync(command);
        }
    }
}

using Convey.CQRS.Commands;
using PizzaItaliano.Services.Releases.Application.Exceptions;
using PizzaItaliano.Services.Releases.Core.Entities;
using PizzaItaliano.Services.Releases.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Application.Commands.Handlers
{
    public class AddReleaseHandler : ICommandHandler<AddRelease>
    {
        private readonly IReleaseRepository _releaseRepository;

        public AddReleaseHandler(IReleaseRepository releaseRepository)
        {
            _releaseRepository = releaseRepository;
        }

        public async Task HandleAsync(AddRelease command)
        {
            var exists = await _releaseRepository.ExistsAsync(command.ReleaseId);

            if (exists)
            {
                throw new ReleaseAlreadyExistsException(command.ReleaseId);
            }

            var release = Release.Create(command.ReleaseId, command.OrderId, command.OrderProductId, DateTime.Now);

            await _releaseRepository.AddAsync(release);
        }
    }
}

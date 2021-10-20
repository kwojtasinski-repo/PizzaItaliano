using Convey.CQRS.Commands;
using PizzaItaliano.Services.Releases.Application.Exceptions;
using PizzaItaliano.Services.Releases.Application.Services;
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
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;

        public AddReleaseHandler(IReleaseRepository releaseRepository, IMessageBroker messageBroker, IEventMapper eventMapper)
        {
            _releaseRepository = releaseRepository;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
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
            var integrationEvents = _eventMapper.MapAll(release.Events);
            await _messageBroker.PublishAsync(integrationEvents);
        }
    }
}

using Convey.CQRS.Commands;
using PizzaItaliano.Services.Releases.Application.Exceptions;
using PizzaItaliano.Services.Releases.Application.Services;
using PizzaItaliano.Services.Releases.Core.Entities;
using PizzaItaliano.Services.Releases.Core.Repositories;
using System;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Application.Commands.Handlers
{
    public class AddReleaseHandler : ICommandHandler<AddRelease>
    {
        private readonly IReleaseRepository _releaseRepository;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;
        private readonly IAppContext _appContext;

        public AddReleaseHandler(IReleaseRepository releaseRepository, IMessageBroker messageBroker, IEventMapper eventMapper, IAppContext appContext)
        {
            _releaseRepository = releaseRepository;
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
            _appContext = appContext;
        }

        public async Task HandleAsync(AddRelease command)
        {
            if (_appContext.Identity.Id == Guid.Empty)
            {
                throw new InvalidUserIdException(_appContext.Identity.Id);
            }

            var exists = await _releaseRepository.ExistsAsync(command.ReleaseId);

            if (exists)
            {
                throw new ReleaseAlreadyExistsException(command.ReleaseId);
            }

            var release = Release.Create(command.ReleaseId, command.OrderId, command.OrderProductId, DateTime.Now, _appContext.Identity.Id);

            await _releaseRepository.AddAsync(release);
            var integrationEvents = _eventMapper.MapAll(release.Events);
            await _messageBroker.PublishAsync(integrationEvents);
        }
    }
}

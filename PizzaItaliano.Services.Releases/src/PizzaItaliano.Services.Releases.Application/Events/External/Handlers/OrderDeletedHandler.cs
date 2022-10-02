using Convey.CQRS.Events;
using PizzaItaliano.Services.Releases.Core.Repositories;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Application.Events.External.Handlers
{
    internal class OrderDeletedHandler : IEventHandler<OrderDeleted>
    {
        private readonly IReleaseRepository _releaseRepository;

        public OrderDeletedHandler(IReleaseRepository releaseRepository)
        {
            _releaseRepository = releaseRepository;
        }

        public async Task HandleAsync(OrderDeleted @event)
        {
            var releases = await _releaseRepository.GetAllByOrderIdAsync(@event.OrderId);

            foreach (var release in releases)
            {
                await _releaseRepository.DeleteAsync(release.Id);
            }
        }
    }
}

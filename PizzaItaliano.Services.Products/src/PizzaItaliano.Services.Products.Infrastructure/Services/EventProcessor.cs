using Convey.CQRS.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PizzaItaliano.Services.Products.Application.Services;
using PizzaItaliano.Services.Products.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Infrastructure.Services
{
    internal sealed class EventProcessor : IEventProcessor // adapter do konwersji DomainEvent na Event i nastepnie przeslania na szyne
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IEventMapper _eventMapper;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<IEventProcessor> _logger;

        public EventProcessor(IMessageBroker messageBroker, IEventMapper eventMapper, IServiceScopeFactory serviceScopeFactory, ILogger<IEventProcessor> logger)
        {
            _messageBroker = messageBroker;
            _eventMapper = eventMapper;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task ProcessAsync(IEnumerable<IDomainEvent> events)
        {
            if (events is null)
            {
                return;
            }

            var integrationEvents = await HandleDomainEventsAsync(events);

            if (!integrationEvents.Any())
            {
                return;
            }

            await _messageBroker.PublishAsync(integrationEvents);
        }

        private async Task<List<IEvent>> HandleDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents)
        {
            var integrationEvents = new List<IEvent>();
            using var scope = _serviceScopeFactory.CreateScope();
            foreach(var domainEvent in domainEvents)
            {
                var eventType = domainEvent.GetType();
                _logger.LogTrace($"Handling a domain event: {eventType.Name}");
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType); // nie mozna podac type jako generic type dlatego trzeba skorzystac z refleksji (MakeGenericType)
                dynamic handlers = scope.ServiceProvider.GetServices(handlerType); // wymagaloby dodatkowego opakowania w klase ktora konwertuje handlers na odpowedni typ, zdecydowalem sie na mniejsza ilosc kodu

                foreach (var handler in handlers)
                {
                    await handler.HandlerAsync((dynamic) domainEvent);
                }

                var integrationEvent = _eventMapper.Map(domainEvent);
                if (integrationEvent is null)
                {
                    continue;
                }

                integrationEvents.Add(integrationEvent);
            }

            return integrationEvents;
        }
    }
}

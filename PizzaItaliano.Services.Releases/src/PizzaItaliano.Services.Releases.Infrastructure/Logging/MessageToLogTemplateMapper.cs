using Convey.Logging.CQRS;
using PizzaItaliano.Services.Releases.Application.Commands;
using PizzaItaliano.Services.Releases.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Infrastructure.Logging
{
    internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
    {
        public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
        {
            var key = message.GetType();

            return Templates.TryGetValue(key, out var template) ? template : null;
        }

        private static readonly IReadOnlyDictionary<Type, HandlerLogTemplate> Templates = new Dictionary<Type, HandlerLogTemplate>()
        {
            [typeof(AddRelease)] = new HandlerLogTemplate
            {
                Before = "Adding an release with id: {ReleaseId}",
                After = "Added an release with id: {ReleaseId}",
                OnError = new Dictionary<Type, string> // w zaleznosci w jakim kontekscie poleci wyjatek
                {
                    [typeof(ReleaseAlreadyExistsException)] = "Release with id: {ReleaseId} already exists"
                }
            }
        };
        }
}

using Convey.CQRS.Commands;
using OpenTracing;
using OpenTracing.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Infrastructure.Tracing
{
    internal sealed class JaegerCommandHandlerDecorator<T> : ICommandHandler<T> where T : class, ICommand
    {
        private readonly ICommandHandler<T> _commandHandler;
        private readonly ITracer _tracer;

        public JaegerCommandHandlerDecorator(ICommandHandler<T> commandHandler, ITracer tracer)
        {
            _commandHandler = commandHandler;
            _tracer = tracer;
        }

        public async Task HandleAsync(T command)
        {
            var commandName = command.GetType().Name;
            using var scope = BuildScope(commandName);
            var span = scope.Span;

            try
            {
                span.Log($"Handling a message: {commandName}");
                await _commandHandler.HandleAsync(command);
                span.Log($"Handled a message: {commandName}");
            }
            catch (Exception ex)
            {
                span.Log($"{ex.Message}");
                span.SetTag(Tags.Error, true);
                throw;
            }
        }

        private IScope BuildScope(string commandName)
        {
            var scope = _tracer.BuildSpan($"handling-{commandName}")
                .WithTag($"message-name", commandName);

            if (_tracer.ActiveSpan is not null)
            {
                scope.AddReference(References.ChildOf, _tracer.ActiveSpan.Context);
            }

            return scope.StartActive(true);
        }
    }
}

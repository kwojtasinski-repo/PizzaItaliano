using Convey.MessageBrokers.RabbitMQ;
using PizzaItaliano.Services.Releases.Application.Commands;
using PizzaItaliano.Services.Releases.Application.Events.Rejected;
using PizzaItaliano.Services.Releases.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Infrastructure.Exceptions
{
    internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
    {
        public object Map(Exception exception, object message)
            =>  exception switch
                {
                    ReleaseAlreadyExistsException ex => message switch
                    {
                        AddRelease command => new AddReleaseRejected(command.ReleaseId, ex.Message, ex.Code),
                        _ => null
                    },

                    _ => null
                };
    }
}

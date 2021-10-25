using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;
using Convey.Docs.Swagger;
using Convey.MessageBrokers.Outbox;
using Convey.MessageBrokers.Outbox.Mongo;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Persistence.MongoDB;
using Convey.WebApi;
using Convey.WebApi.Swagger;
using Convey.WebApi.CQRS;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Releases.Application;
using PizzaItaliano.Services.Releases.Application.Services;
using PizzaItaliano.Services.Releases.Core.Repositories;
using PizzaItaliano.Services.Releases.Infrastructure.Decorators;
using PizzaItaliano.Services.Releases.Infrastructure.Exceptions;
using PizzaItaliano.Services.Releases.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Releases.Infrastructure.Mongo.Repositories;
using PizzaItaliano.Services.Releases.Infrastructure.Services;
using System;
using Convey.MessageBrokers.CQRS;
using PizzaItaliano.Services.Releases.Application.Commands;

namespace PizzaItaliano.Services.Releases.Infrastructure
{
    public static class Extensions
    {
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder conveyBuilder)
        {
            conveyBuilder.Services.AddTransient<IReleaseRepository, ReleaseRepository>();
            conveyBuilder.Services.AddTransient<IMessageBroker, MessageBroker>();
            conveyBuilder.Services.AddSingleton<IEventMapper, EventMapper>();
            conveyBuilder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(OutboxCommandHandlerDecorator<>));
            conveyBuilder.Services.TryDecorate(typeof(IEventHandler<>), typeof(OutboxEventHandlerDecorator<>));

            conveyBuilder.AddErrorHandler<ExceptionToResponseMapper>();
            conveyBuilder.AddExceptionToMessageMapper<ExceptionToMessageMapper>();
            conveyBuilder.AddQueryHandlers();
            conveyBuilder.AddInMemoryQueryDispatcher();
            conveyBuilder.AddMongo();
            conveyBuilder.AddMongoRepository<ReleaseDocument, Guid>("releases");
            conveyBuilder.AddSwaggerDocs();
            conveyBuilder.AddWebApiSwaggerDocs();
            conveyBuilder.AddMessageOutbox(o => o.AddMongo());
            conveyBuilder.AddRabbitMq();

            return conveyBuilder;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler()
               .UseConvey()
               .UsePublicContracts<ContractAttribute>()
               .UseSwaggerDocs()
               .UseRabbitMq()
               .SubscribeCommand<AddRelease>();

            return app;
        }
    }
}

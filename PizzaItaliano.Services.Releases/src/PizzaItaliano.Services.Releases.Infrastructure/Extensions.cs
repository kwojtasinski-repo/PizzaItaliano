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
using Convey.LoadBalancing.Fabio;
using Convey.Discovery.Consul;
using System.Runtime.CompilerServices;
using PizzaItaliano.Services.Releases.Infrastructure.Logging;
using Convey.Metrics.AppMetrics;
using PizzaItaliano.Services.Releases.Infrastructure.Metrics;
using Convey.Tracing.Jaeger;
using PizzaItaliano.Services.Releases.Infrastructure.Tracing;
using Convey.Tracing.Jaeger.RabbitMQ;

[assembly: InternalsVisibleTo("PizzaItaliano.Services.Releases.Tests.EndToEnd")] // widocznosc internal na poziomie testow (end-to-end)
[assembly: InternalsVisibleTo("PizzaItaliano.Services.Releases.Tests.Intgration")] // widocznosc internal na poziomie testow (integration)
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
            conveyBuilder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(JaegerCommandHandlerDecorator<>));

            conveyBuilder.Services.AddHostedService<MetricsJob>();
            conveyBuilder.Services.AddSingleton<CustomMetricsMiddleware>();

            conveyBuilder.AddErrorHandler<ExceptionToResponseMapper>();
            conveyBuilder.AddExceptionToMessageMapper<ExceptionToMessageMapper>();
            conveyBuilder.AddQueryHandlers();
            conveyBuilder.AddInMemoryQueryDispatcher();
            conveyBuilder.AddMongo();
            conveyBuilder.AddMongoRepository<ReleaseDocument, Guid>("releases");
            conveyBuilder.AddSwaggerDocs();
            conveyBuilder.AddWebApiSwaggerDocs();
            conveyBuilder.AddMessageOutbox(o => o.AddMongo());
            conveyBuilder.AddRabbitMq(plugins: p => p.AddJaegerRabbitMqPlugin());
            conveyBuilder.AddConsul();
            conveyBuilder.AddFabio();
            conveyBuilder.AddHandlersLogging();
            conveyBuilder.AddMetrics();
            conveyBuilder.AddJaeger();

            return conveyBuilder;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler()
               .UseConvey()
               .UseJaeger()
               .UsePublicContracts<ContractAttribute>()
               .UseSwaggerDocs()
               .UseMetrics()
               .UseMiddleware<CustomMetricsMiddleware>()
               .UseRabbitMq()
               .SubscribeCommand<AddRelease>();

            return app;
        }
    }
}

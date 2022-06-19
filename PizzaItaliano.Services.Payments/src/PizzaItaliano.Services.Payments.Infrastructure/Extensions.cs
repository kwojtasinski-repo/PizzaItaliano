﻿using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;
using Convey.Discovery.Consul;
using Convey.Docs.Swagger;
using Convey.HTTP;
using Convey.LoadBalancing.Fabio;
using Convey.MessageBrokers.CQRS;
using Convey.MessageBrokers.Outbox;
using Convey.MessageBrokers.Outbox.Mongo;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Metrics.AppMetrics;
using Convey.Persistence.MongoDB;
using Convey.Tracing.Jaeger;
using Convey.Tracing.Jaeger.RabbitMQ;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Convey.WebApi.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Payments.Application;
using PizzaItaliano.Services.Payments.Application.Commands;
using PizzaItaliano.Services.Payments.Application.Events.External;
using PizzaItaliano.Services.Payments.Application.Services;
using PizzaItaliano.Services.Payments.Application.Services.Clients;
using PizzaItaliano.Services.Payments.Core.Repositories;
using PizzaItaliano.Services.Payments.Infrastructure.Decorators;
using PizzaItaliano.Services.Payments.Infrastructure.Exceptions;
using PizzaItaliano.Services.Payments.Infrastructure.Logging;
using PizzaItaliano.Services.Payments.Infrastructure.Metrics;
using PizzaItaliano.Services.Payments.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Payments.Infrastructure.Repositories;
using PizzaItaliano.Services.Payments.Infrastructure.Services;
using PizzaItaliano.Services.Payments.Infrastructure.Services.Clients;
using PizzaItaliano.Services.Payments.Infrastructure.Tracing;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PizzaItaliano.Services.Payments.Tests.EndToEnd")] // widocznosc internal na poziomie testow (end-to-end)
[assembly: InternalsVisibleTo("PizzaItaliano.Services.Payments.Tests.Intgration")] // widocznosc internal na poziomie testow (integration)
namespace PizzaItaliano.Services.Payments.Infrastructure
{
    public static class Extensions
    {
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder conveyBuilder)
        {
            conveyBuilder.Services.AddTransient<IPaymentRepository, PaymentRepository>();
            conveyBuilder.Services.AddTransient<IMessageBroker, MessageBroker>();
            conveyBuilder.Services.AddSingleton<IEventMapper, EventMapper>();
            conveyBuilder.Services.AddTransient<IOrderServiceClient, OrderServiceClient>();
            conveyBuilder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(OutboxCommandHandlerDecorator<>));
            conveyBuilder.Services.TryDecorate(typeof(IEventHandler<>), typeof(OutboxEventHandlerDecorator<>));
            conveyBuilder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(JaegerCommandHandlerDecorator<>));

            conveyBuilder.Services.AddHostedService<MetricsJob>();
            conveyBuilder.Services.AddSingleton<CustomMetricsMiddleware>();

            conveyBuilder.AddErrorHandler<ExceptionToResponseMapper>();
            conveyBuilder.AddMessageOutbox(o => o.AddMongo());
            conveyBuilder.AddExceptionToMessageMapper<ExceptionToMessageMapper>();
            conveyBuilder.AddQueryHandlers();
            conveyBuilder.AddInMemoryQueryDispatcher();
            conveyBuilder.AddHttpClient();
            conveyBuilder.AddMongo();
            conveyBuilder.AddMongoRepository<PaymentDocument, Guid>("payments");
            conveyBuilder.AddSwaggerDocs();
            conveyBuilder.AddWebApiSwaggerDocs();
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
               .SubscribeCommand<AddPayment>()
               .SubscribeCommand<UpdatePayment>();
               //.SubscribeEvent<OrderStateModified>(); // wywola event handler aktualnie nie jest potrzebny event

            return app;
        }
    }
}

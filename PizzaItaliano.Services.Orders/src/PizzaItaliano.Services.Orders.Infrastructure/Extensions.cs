﻿using Convey;
using Convey.CQRS.Queries;
using Convey.Docs.Swagger;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Persistence.MongoDB;
using Convey.WebApi;
using Convey.WebApi.Swagger;
using Convey.WebApi.CQRS;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Orders.Application;
using PizzaItaliano.Services.Orders.Application.Services;
using PizzaItaliano.Services.Orders.Core.Repositories;
using PizzaItaliano.Services.Orders.Infrastructure.Exceptions;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Orders.Infrastructure.Repositories;
using PizzaItaliano.Services.Orders.Infrastructure.Services;
using System;
using PizzaItaliano.Services.Orders.Infrastructure.Services.Clients;
using PizzaItaliano.Services.Orders.Application.Services.Clients;
using Convey.HTTP;
using PizzaItaliano.Services.Orders.Application.Events.External;
using Convey.MessageBrokers.CQRS;
using Convey.CQRS.Commands;
using PizzaItaliano.Services.Orders.Infrastructure.Decorators;
using Convey.CQRS.Events;
using Convey.MessageBrokers.Outbox;
using Convey.MessageBrokers.Outbox.Mongo;
using PizzaItaliano.Services.Orders.Application.Commands;
using Convey.Discovery.Consul;
using Convey.LoadBalancing.Fabio;
using System.Runtime.CompilerServices;
using Convey.Persistence.Redis;
using PizzaItaliano.Services.Orders.Infrastructure.Types;
using PizzaItaliano.Services.Orders.Infrastructure.Logging;
using Convey.Metrics.AppMetrics;
using PizzaItaliano.Services.Orders.Infrastructure.Metrics;
using Convey.Tracing.Jaeger;
using PizzaItaliano.Services.Orders.Infrastructure.Tracing;
using Convey.Tracing.Jaeger.RabbitMQ;
using PizzaItaliano.Services.Orders.Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;

[assembly: InternalsVisibleTo("PizzaItaliano.Services.Orders.Tests.EndToEnd")] // widocznosc internal na poziomie testow (end-to-end)
[assembly: InternalsVisibleTo("PizzaItaliano.Services.Orders.Tests.Integration")] // widocznosc internal na poziomie testow (integration)
namespace PizzaItaliano.Services.Orders.Infrastructure
{
    public static class Extensions
    {
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder conveyBuilder)
        {
            var requestsOptions = conveyBuilder.GetOptions<RequestsOptions>("requests");
            conveyBuilder.Services.AddSingleton(requestsOptions);
            conveyBuilder.Services.AddTransient<IOrderRepository, OrderRepository>();
            conveyBuilder.Services.AddTransient<IMessageBroker, MessageBroker>();
            conveyBuilder.Services.AddSingleton<IEventMapper, EventMapper>();
            conveyBuilder.Services.AddTransient<IProductServiceClient, ProductServiceClient>();
            conveyBuilder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(OutboxCommandHandlerDecorator<>));
            conveyBuilder.Services.TryDecorate(typeof(IEventHandler<>), typeof(OutboxEventHandlerDecorator<>));
            conveyBuilder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(JaegerCommandHandlerDecorator<>));

            conveyBuilder.Services.AddTransient<IAppContextFactory, AppContextFactory>();
            conveyBuilder.Services.AddTransient(ctx => ctx.GetRequiredService<IAppContextFactory>().Create());

            conveyBuilder.Services.AddHostedService<MetricsJob>();
            conveyBuilder.Services.AddSingleton<CustomMetricsMiddleware>();

            conveyBuilder.AddErrorHandler<ExceptionToResponseMapper>();
            conveyBuilder.AddMessageOutbox(o => o.AddMongo());
            conveyBuilder.AddExceptionToMessageMapper<ExceptionToMessageMapper>();
            conveyBuilder.AddQueryHandlers();
            conveyBuilder.AddInMemoryQueryDispatcher();
            conveyBuilder.AddHttpClient();
            conveyBuilder.AddMongo();
            conveyBuilder.AddMongoRepository<OrderDocument, Guid>("orders");
            conveyBuilder.AddSwaggerDocs();
            conveyBuilder.AddWebApiSwaggerDocs();
            conveyBuilder.AddRabbitMq(plugins: p => p.AddJaegerRabbitMqPlugin());
            conveyBuilder.AddConsul();
            conveyBuilder.AddFabio();
            conveyBuilder.AddRedis();
            conveyBuilder.AddSignalR();
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
               .SubscribeCommand<AddOrder>()
               .SubscribeCommand<AddOrderProduct>()
               .SubscribeCommand<DeleteOrderProduct>()
               .SubscribeCommand<SetOrderStatusReady>()
               .SubscribeEvent<PaidPayment>()
               .SubscribeEvent<ReleaseAdded>() 
               .SubscribeEvent<WithdrawnPayment>(); 

            return app;
        }

        private static IConveyBuilder AddSignalR(this IConveyBuilder builder)
        {
            var options = builder.GetOptions<SignalrOptions>("signalR");
            builder.Services.AddSingleton(options);
            var signalR = builder.Services.AddSignalR();
            if (!options.Backplane.Equals("redis", StringComparison.InvariantCultureIgnoreCase))
            {
                return builder;
            }

            var redisOptions = builder.GetOptions<RedisOptions>("redis");
            signalR.AddRedis(redisOptions.ConnectionString);

            return builder;
        }

        internal static CorrelationContext GetCorrelationContext(this IHttpContextAccessor accessor)
        {
            if (accessor.HttpContext is null)
            {
                return null;
            }

            var parsed = accessor.HttpContext.Request.Headers.TryGetValue("Correlation-Context", out var json);

            if (!parsed)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<CorrelationContext>(json.FirstOrDefault());
        }
    }
}

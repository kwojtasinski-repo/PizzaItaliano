using Convey;
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
using PizzaItaliano.Services.Orders.Application.Events;
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

namespace PizzaItaliano.Services.Orders.Infrastructure
{
    public static class Extensions
    {
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder conveyBuilder)
        {
            conveyBuilder.Services.AddTransient<IOrderRepository, OrderRepository>();
            conveyBuilder.Services.AddTransient<IMessageBroker, MessageBroker>();
            conveyBuilder.Services.AddSingleton<IEventMapper, EventMapper>();
            conveyBuilder.Services.AddTransient<IProductServiceClient, ProductServiceClient>();
            conveyBuilder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(OutboxCommandHandlerDecorator<>));
            conveyBuilder.Services.TryDecorate(typeof(IEventHandler<>), typeof(OutboxEventHandlerDecorator<>));

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
            conveyBuilder.AddRabbitMq();
            conveyBuilder.AddConsul();
            conveyBuilder.AddFabio();

            return conveyBuilder;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler()
               .UseConvey()
               .UsePublicContracts<ContractAttribute>()
               .UseSwaggerDocs()
               .UseRabbitMq()
               .SubscribeCommand<AddOrder>()
               .SubscribeCommand<AddOrderProduct>()
               .SubscribeCommand<DeleteOrderProduct>()
               .SubscribeCommand<SetOrderStatusReady>()
               .SubscribeEvent<PaidPayment>()
               .SubscribeEvent<ReleaseAdded>(); 

            return app;
        }
    }
}

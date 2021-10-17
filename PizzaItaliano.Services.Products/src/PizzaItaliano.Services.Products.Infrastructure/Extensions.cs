using Convey;
using Convey.Persistence.MongoDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Products.Core.Repositories;
using PizzaItaliano.Services.Products.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Products.Infrastructure.Mongo.Repositories;
using Convey.WebApi.Swagger;
using Convey.Docs.Swagger;
using System;
using Convey.CQRS.Queries;
using Convey.MessageBrokers.RabbitMQ;
using Convey.WebApi;
using PizzaItaliano.Services.Products.Infrastructure.Exceptions;
using PizzaItaliano.Services.Products.Application;
using Convey.WebApi.CQRS;
using PizzaItaliano.Services.Products.Infrastructure.Services;
using PizzaItaliano.Services.Products.Application.Services;

namespace PizzaItaliano.Services.Products.Infrastructure
{
    public static class Extensions
    {
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder conveyBuilder)
        {
            conveyBuilder.Services.AddTransient<IProductRepository, ProductRepository>();
            conveyBuilder.Services.AddTransient<IMessageBroker, MessageBroker>();
            conveyBuilder.Services.AddTransient<IEventProcessor, EventProcessor>();
            conveyBuilder.Services.AddSingleton<IEventMapper, EventMapper>();

            conveyBuilder.Services.Scan(s => s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()); // przeszukaj Assembly i znajdz wszystkie klasy ktore implementuja IDomainEventHandler jako zaimplementowane interfejsy z cyklem zycia Transient

            conveyBuilder.AddErrorHandler<ExceptionToResponseMapper>();
            conveyBuilder.AddExceptionToMessageMapper<ExceptionToMessageMapper>();
            conveyBuilder.AddQueryHandlers();
            conveyBuilder.AddInMemoryQueryDispatcher();
            conveyBuilder.AddMongo();
            conveyBuilder.AddMongoRepository<ProductDocument, Guid>("products");
            conveyBuilder.AddSwaggerDocs();
            conveyBuilder.AddWebApiSwaggerDocs();
            conveyBuilder.AddRabbitMq();

            return conveyBuilder;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler()
               .UseConvey()
               .UsePublicContracts<ContractAttribute>()
               .UseSwaggerDocs();

            return app;
        }
    }
}

using Convey;
using Convey.CQRS.Queries;
using Convey.Docs.Swagger;
using Convey.Persistence.MongoDB;
using Convey.WebApi;
using Convey.WebApi.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Orders.Core.Repositories;
using PizzaItaliano.Services.Orders.Infrastructure.Exceptions;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Orders.Infrastructure.Repositories;
using System;

namespace PizzaItaliano.Services.Orders.Infrastructure
{
    public static class Extensions
    {
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder conveyBuilder)
        {
            conveyBuilder.Services.AddTransient<IOrderRepository, OrderRepository>();

            conveyBuilder.AddErrorHandler<ExceptionToResponseMapper>();
            conveyBuilder.AddQueryHandlers();
            conveyBuilder.AddInMemoryQueryDispatcher();
            conveyBuilder.AddMongo();
            conveyBuilder.AddMongoRepository<OrderDocument, Guid>("orders");
            conveyBuilder.AddSwaggerDocs();
            conveyBuilder.AddWebApiSwaggerDocs();

            return conveyBuilder;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler()
               .UseConvey()
               .UseSwaggerDocs();

            return app;
        }
    }
}

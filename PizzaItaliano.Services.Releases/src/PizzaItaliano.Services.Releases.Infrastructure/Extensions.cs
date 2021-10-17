using Convey;
using Convey.CQRS.Queries;
using Convey.Docs.Swagger;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Persistence.MongoDB;
using Convey.WebApi;
using Convey.WebApi.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Releases.Core.Repositories;
using PizzaItaliano.Services.Releases.Infrastructure.Exceptions;
using PizzaItaliano.Services.Releases.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Releases.Infrastructure.Mongo.Repositories;
using System;

namespace PizzaItaliano.Services.Releases.Infrastructure
{
    public static class Extensions
    {
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder conveyBuilder)
        {
            conveyBuilder.Services.AddTransient<IReleaseRepository, ReleaseRepository>();

            conveyBuilder.AddErrorHandler<ExceptionToResponseMapper>();
            conveyBuilder.AddQueryHandlers();
            conveyBuilder.AddInMemoryQueryDispatcher();
            conveyBuilder.AddMongo();
            conveyBuilder.AddMongoRepository<ReleaseDocument, Guid>("releases");
            conveyBuilder.AddSwaggerDocs();
            conveyBuilder.AddWebApiSwaggerDocs();
            conveyBuilder.AddRabbitMq();

            return conveyBuilder;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler()
               .UseConvey()
               .UseSwaggerDocs()
               .UseRabbitMq();

            return app;
        }
    }
}

using Convey;
using Convey.CQRS.Queries;
using Convey.Docs.Swagger;
using Convey.Persistence.MongoDB;
using Convey.WebApi;
using Convey.WebApi.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Payments.Core.Repositories;
using PizzaItaliano.Services.Payments.Infrastructure.Exceptions;
using PizzaItaliano.Services.Payments.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Payments.Infrastructure.Repositories;
using System;

namespace PizzaItaliano.Services.Payments.Infrastructure
{
    public static class Extensions
    {
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder conveyBuilder)
        {
            conveyBuilder.Services.AddTransient<IPaymentRepository, PaymentRepository>();

            conveyBuilder.AddErrorHandler<ExceptionToResponseMapper>();
            conveyBuilder.AddQueryHandlers();
            conveyBuilder.AddInMemoryQueryDispatcher();
            conveyBuilder.AddMongo();
            conveyBuilder.AddMongoRepository<PaymentDocument, Guid>("payments");
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

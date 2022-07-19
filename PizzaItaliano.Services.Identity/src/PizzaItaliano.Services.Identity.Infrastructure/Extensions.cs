using Convey;
using Convey.Auth;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;
using Convey.Discovery.Consul;
using Convey.Docs.Swagger;
using Convey.HTTP;
using Convey.LoadBalancing.Fabio;
using Convey.MessageBrokers;
using Convey.MessageBrokers.CQRS;
using Convey.MessageBrokers.Outbox;
using Convey.MessageBrokers.Outbox.Mongo;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Metrics.AppMetrics;
using Convey.Persistence.MongoDB;
using Convey.Security;
using Convey.Tracing.Jaeger;
using Convey.Tracing.Jaeger.RabbitMQ;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Convey.WebApi.Swagger;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PizzaItaliano.Services.Identity.Application;
using PizzaItaliano.Services.Identity.Application.Commands;
using PizzaItaliano.Services.Identity.Application.Services;
using PizzaItaliano.Services.Identity.Application.Services.Identity;
using PizzaItaliano.Services.Identity.Core.Repositories;
using PizzaItaliano.Services.Identity.Infrastructure.Auth;
using PizzaItaliano.Services.Identity.Infrastructure.Contexts;
using PizzaItaliano.Services.Identity.Infrastructure.Decorators;
using PizzaItaliano.Services.Identity.Infrastructure.Exceptions;
using PizzaItaliano.Services.Identity.Infrastructure.Mongo;
using PizzaItaliano.Services.Identity.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Identity.Infrastructure.Mongo.Repositories;
using PizzaItaliano.Services.Identity.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("PizzaItaliano.Services.Identity.Tests.Integration")]
[assembly: InternalsVisibleTo("PizzaItaliano.Services.Identity.Tests.EndToEnd")]
namespace PizzaItaliano.Services.Identity.Infrastructure
{
    public static class Extensions
    {
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder conveyBuilder)
        {
            conveyBuilder.Services.AddSingleton<IJwtProvider, JwtProvider>();
            conveyBuilder.Services.AddSingleton<IPasswordService, PasswordService>();
            conveyBuilder.Services.AddSingleton<IPasswordHasher<IPasswordService>, PasswordHasher<IPasswordService>>();
            conveyBuilder.Services.AddTransient<IIdentityService, IdentityService>();
            conveyBuilder.Services.AddTransient<IRefreshTokenService, RefreshTokenService>();
            conveyBuilder.Services.AddSingleton<IRng, Rng>();
            conveyBuilder.Services.AddTransient<IMessageBroker, MessageBroker>();
            conveyBuilder.Services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            conveyBuilder.Services.AddTransient<IUserRepository, UserRepository>();
            conveyBuilder.Services.AddTransient<IAppContextFactory, AppContextFactory>();
            conveyBuilder.Services.AddTransient(ctx => ctx.GetRequiredService<IAppContextFactory>().Create());
            conveyBuilder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(OutboxCommandHandlerDecorator<>));
            conveyBuilder.Services.TryDecorate(typeof(IEventHandler<>), typeof(OutboxEventHandlerDecorator<>));

            conveyBuilder.AddErrorHandler<ExceptionToResponseMapper>();
            conveyBuilder.AddQueryHandlers();
            conveyBuilder.AddInMemoryQueryDispatcher();
            conveyBuilder.AddJwt();
            conveyBuilder.AddHttpClient();
            conveyBuilder.AddConsul();
            conveyBuilder.AddFabio();
            conveyBuilder.AddExceptionToMessageMapper<ExceptionToMessageMapper>();
            conveyBuilder.AddRabbitMq(plugins: p => p.AddJaegerRabbitMqPlugin());
            conveyBuilder.AddMessageOutbox(o => o.AddMongo());
            conveyBuilder.AddMongo();
            conveyBuilder.AddMetrics();
            conveyBuilder.AddJaeger();
            conveyBuilder.AddMongoRepository<RefreshTokenDocument, Guid>("refreshTokens");
            conveyBuilder.AddMongoRepository<UserDocument, Guid>("users");
            conveyBuilder.AddWebApiSwaggerDocs();
            conveyBuilder.AddSecurity();

            return conveyBuilder;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler();
            app.UseSwaggerDocs();
            app.UseJaeger();
            app.UseConvey();
            app.UseAccessTokenValidator();
            app.UseMongo();
            app.UsePublicContracts<ContractAttribute>();
            app.UseMetrics();
            app.UseAuthentication();
            app.UseRabbitMq()
                .SubscribeCommand<SignUp>();

            return app;
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
        internal static IDictionary<string, object> GetHeadersToForward(this IMessageProperties messageProperties)
        {
            const string sagaHeader = "Saga";
            if (messageProperties?.Headers is null || !messageProperties.Headers.TryGetValue(sagaHeader, out var saga))
            {
                return null;
            }

            return saga is null
                ? null
                : new Dictionary<string, object>
                {
                    [sagaHeader] = saga
                };
        }

        internal static string GetSpanContext(this IMessageProperties messageProperties, string header)
        {
            if (messageProperties is null)
            {
                return string.Empty;
            }

            if (messageProperties.Headers.TryGetValue(header, out var span) && span is byte[] spanBytes)
            {
                return Encoding.UTF8.GetString(spanBytes);
            }

            return string.Empty;
        }

        public static async Task<Guid> AuthenticateUsingJwtAsync(this HttpContext context)
        {
            var authentication = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            return authentication.Succeeded ? Guid.Parse(authentication.Principal.Identity.Name) : Guid.Empty;
        }
    }
}
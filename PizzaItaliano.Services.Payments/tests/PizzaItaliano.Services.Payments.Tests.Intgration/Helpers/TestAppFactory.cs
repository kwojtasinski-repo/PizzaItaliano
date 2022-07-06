using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Payments.API;
using PizzaItaliano.Services.Payments.Application;
using PizzaItaliano.Services.Payments.Infrastructure.Contexts;
using PizzaItaliano.Services.Payments.Tests.Shared.Factories;
using System;
using System.Collections.Generic;

namespace PizzaItaliano.Services.Payments.Tests.Intgration.Helpers
{
    public class TestAppFactory : PizzaItalianoApplicationFactory<Program>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = base.CreateWebHostBuilder();
            builder.ConfigureServices(services =>
                    services.AddTransient<IAppContext>(app =>
                    {
                        return new Infrastructure.Contexts.AppContext(new CorrelationContext
                        {
                            ConnectionId = Guid.NewGuid().ToString("N"),
                            CorrelationId = Guid.NewGuid().ToString("N"),
                            CreatedAt = DateTime.Now.AddSeconds(-20.5),
                            Name = Guid.NewGuid().ToString("N"),
                            ResourceId = Guid.NewGuid().ToString("N"),
                            SpanContext = Guid.NewGuid().ToString("N"),
                            TraceId = Guid.NewGuid().ToString("N"),
                            User = new CorrelationContext.UserContext
                            {
                                Id = Guid.NewGuid().ToString(),
                                IsAuthenticated = true,
                                Role = "admin",
                                Claims = new Dictionary<string, string>(),
                            }
                        });
                    }));
            return builder;
        }
    }
}
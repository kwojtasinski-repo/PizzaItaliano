using System;
using System.IO;
using System.Threading.Tasks;
using Convey;
using Convey.Logging;
using Convey.Types;
using Convey.WebApi;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using PizzaItaliano.Services.Operations.Hubs;
using PizzaItaliano.Services.Operations.Infrastructure;
using PizzaItaliano.Services.Operations.Queries;
using PizzaItaliano.Services.Operations.Services;

namespace PizzaItaliano.Services.Operations
{
    public class Program
    {
        public static async Task Main(string[] args)
            => await WebHost.CreateDefaultBuilder(args)
                    .ConfigureServices(services => services
                        .AddConvey()
                        .AddWebApi()
                        .AddInfrastructure()
                        .Build()
                    )
                    .Configure(app => app
                        .UseInfrastructure()
                        .UseEndpoints(endpoints => endpoints
                            .Get("", ctx => ctx.Response.WriteAsJsonAsync(ctx.RequestServices.GetService<AppOptions>().Name))
                            .Get<GetOperation>("operations/{operationId}", async (query, ctx) =>
                            {
                                var operation = await ctx.RequestServices.GetService<IOperationsService>()
                                                    .GetAsync(query.OperationId);

                                if (operation is null)
                                {
                                    await ctx.Response.NotFound();
                                    return;
                                }

                                await ctx.Response.WriteJsonAsync(operation);
                            }))
                    // gRPC  Protocol
                        .UseEndpoints(endpoints =>
                        {
                            endpoints.MapHub<PizzaItalianoHub>("/pizza-italiano");
                            endpoints.MapGrpcService<GrpcServiceHost>();
                        })
                    )
                    .UseLogging()
                    .Build()
                    .RunAsync();
    }
}

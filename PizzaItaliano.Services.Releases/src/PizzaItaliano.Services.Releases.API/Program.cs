using System.Collections.Generic;
using System.Threading.Tasks;
using Convey;
using Convey.Logging;
using Convey.Types;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Releases.Application;
using PizzaItaliano.Services.Releases.Infrastructure;
using PizzaItaliano.Services.Releases.Application.DTO;
using PizzaItaliano.Services.Releases.Application.Queries;
using PizzaItaliano.Services.Releases.Application.Commands;
using Convey.Secrets.Vault;
using System.Net;

namespace PizzaItaliano.Services.Releases.API
{
    public class Program
    {
        public static async Task Main(string[] args)
            => await CreateWebHostBuilder(args).Build().RunAsync();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
            => WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                    .AddConvey()
                    .AddWebApi()
                    .AddApplication()
                    .AddInfrastructure()
                    .Build())
                .Configure(app => app
                    .UseInfrastructure()
                    .UseDispatcherEndpoints(endpoints => endpoints
                        .Get("", ctx => ctx.Response.WriteAsync(ctx.RequestServices.GetService<AppOptions>().Name))
                        .Get<GetReleases, IEnumerable<ReleaseDto>>("releases")
                        .Get<GetRelease, ReleaseDto>("releases/{releaseId}", afterDispatch: (cmd, result, ctx) =>
                        {
                            if (result is null)
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                var task = ctx.Response.WriteAsJsonAsync(new
                                {
                                    code = (int)HttpStatusCode.NotFound,
                                    reason = $"Release with id {cmd.ReleaseId} was not found"
                                });

                                return task;
                            }

                            return ctx.Response.Ok(result);
                        })
                        .Get<GetReleases, IEnumerable<ReleaseDto>>("releases/by-order/{orderId}")
                        .Post<AddRelease>("releases", afterDispatch: (cmd, ctx) => ctx.Response.Created($"releases/{cmd.ReleaseId}"))
                    ))
                .UseLogging()
                .UseVault();
    }
}

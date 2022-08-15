using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Convey;
using Convey.Logging;
using Convey.Secrets.Vault;
using Convey.Types;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Products.Application;
using PizzaItaliano.Services.Products.Application.Commands;
using PizzaItaliano.Services.Products.Application.DTO;
using PizzaItaliano.Services.Products.Application.Exceptions;
using PizzaItaliano.Services.Products.Application.Queries;
using PizzaItaliano.Services.Products.Infrastructure;

namespace PizzaItaliano.Services.Products.API
{
    public class Program
    {
        public static async Task Main(string[] args)
            =>  await CreateWebHostBuilder(args).Build().RunAsync();

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
                        .Get("", ctx => ctx.Response.WriteAsJsonAsync(ctx.RequestServices.GetService<AppOptions>().Name))
                        .Get<GetProducts, IEnumerable<ProductDto>>("products")
                        .Get<GetProducts, IEnumerable<ProductDto>>("products/name/{name}")
                        .Get<GetProduct, ProductDto>("products/{productId}", afterDispatch: (cmd, result, ctx) =>
                        {
                            if (result is null)
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                var task = ctx.Response.WriteAsJsonAsync(new
                                {
                                    code = (int)HttpStatusCode.NotFound,
                                    reason = $"Product with id {cmd.ProductId} was not found"
                                });

                                return task;
                            }

                            return ctx.Response.Ok(result);
                        })
                        .Post<AddProduct>("products", afterDispatch: (cmd, ctx) => ctx.Response.Created($"products/{cmd.ProductId}"))
                        .Put<UpdateProduct>("products/{productId}", beforeDispatch: async (cmd, ctx) =>
                        {
                            var isValid = Guid.TryParse(ctx.Request.RouteValues["productId"] as string, out var productId);

                            if (!isValid)
                            {
                                throw new InvalidProductIdException();
                            }

                            cmd.ProductId = productId;
                        }, afterDispatch: (cmd, ctx) => ctx.Response.Ok($"products/{cmd.ProductId}"))
                        .Delete<DeleteProduct>("products/{productId}", afterDispatch: (cmd, ctx) => ctx.Response.Ok($"Product with {cmd.ProductId} deleted"))
                    ))
                .UseLogging()
                .UseVault();
    }
}

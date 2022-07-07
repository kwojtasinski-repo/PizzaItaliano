using Convey;
using Convey.Auth;
using Convey.Logging;
using Convey.Secrets.Vault;
using Convey.Types;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PizzaItaliano.Services.Identity.Application;
using PizzaItaliano.Services.Identity.Application.Commands;
using PizzaItaliano.Services.Identity.Application.DTO;
using PizzaItaliano.Services.Identity.Application.Exceptions;
using PizzaItaliano.Services.Identity.Application.Queries;
using PizzaItaliano.Services.Identity.Application.Services;
using PizzaItaliano.Services.Identity.Infrastructure;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity
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
                            .Get("", ctx => ctx.Response.WriteAsJsonAsync(ctx.RequestServices.GetService<AppOptions>().Name))
                            .Get<GetUser, UserDto>("users/{userId}", beforeDispatch: async (cmd, ctx) =>
                            {
                                var userId = await ctx.AuthenticateUsingJwtAsync();
                                if (userId == Guid.Empty)
                                {
                                    throw new UnauthorizedException();
                                }
                            }, 
                            afterDispatch: (cmd, result, ctx) =>
                            {
                                if (result is null)
                                {
                                    ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                    var task = ctx.Response.WriteAsJsonAsync(new
                                    {
                                        code = (int)HttpStatusCode.NotFound,
                                        reason = $"User with id {cmd.UserId} was not found"
                                    });

                                    return task;
                                }

                                return ctx.Response.Ok(result);
                            })
                            .Get("me", async ctx =>
                            {
                                var userId = await ctx.AuthenticateUsingJwtAsync();
                                if (userId == Guid.Empty)
                                {
                                    ctx.Response.StatusCode = 401;
                                    return;
                                }

                                await GetUserAsync(userId, ctx);
                            })
                            .Post<SignIn>("sign-in", afterDispatch: async (cmd, ctx) =>
                            {
                                var token = await ctx.RequestServices.GetService<IIdentityService>().SignInAsync(cmd);
                                await ctx.Response.WriteAsJsonAsync(token);
                            })
                            .Post<SignUp>("sign-up", afterDispatch: (cmd, ctx) => ctx.Response.Created("identity/me"))
                            .Post<RevokeAccessToken>("access-tokens/revoke", async (cmd, ctx) =>
                            {
                                await ctx.RequestServices.GetService<IAccessTokenService>().DeactivateAsync(cmd.AccessToken);
                            })
                            .Post<UseRefreshToken>("refresh-tokens/use", afterDispatch: async (cmd, ctx) =>
                            {
                                var token = await ctx.RequestServices.GetService<IRefreshTokenService>().UseAsync(cmd.RefreshToken);
                                await ctx.Response.WriteJsonAsync(token);
                            })
                            .Post<RevokeRefreshToken>("refresh-tokens/revoke", afterDispatch: async (cmd, ctx) =>
                            {
                                await ctx.RequestServices.GetService<IRefreshTokenService>().RevokeAsync(cmd.RefreshToken);
                            })
                        ))
                    .UseLogging()
                    .UseVault();

        private static async Task GetUserAsync(Guid id, HttpContext context)
        {
            var user = await context.RequestServices.GetService<IIdentityService>().GetAsync(id);
            if (user is null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            await context.Response.WriteJsonAsync(user);
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Tests.Shared.Factories
{
    public class PizzaItalianoApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        public TestServer GetTestServer() // fix net 5.0
        {
            var server = new TestServer(CreateWebHostBuilder());
            return server;
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
            => base.CreateWebHostBuilder().UseEnvironment("tests");
    }
}
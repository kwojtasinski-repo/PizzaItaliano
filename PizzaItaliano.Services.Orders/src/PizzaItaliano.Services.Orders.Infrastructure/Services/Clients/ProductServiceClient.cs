using Convey.HTTP;
using PizzaItaliano.Services.Orders.Application.DTO;
using PizzaItaliano.Services.Orders.Application.Services.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Infrastructure.Services.Clients
{
    internal sealed class ProductServiceClient : IProductServiceClient
    {
        private readonly IHttpClient _httpClient;
        private readonly string _url;

        public ProductServiceClient(IHttpClient httpClient, HttpClientOptions httpClientOptions)
        {
            _httpClient = httpClient;
            _url = httpClientOptions.Services["products"];
        }

        public async Task<ProductDto> GetAsync(Guid id)
        {
            var product = await _httpClient.GetAsync<ProductDto>($"{_url}/products/{id}");
            return product;
        }
    }
}

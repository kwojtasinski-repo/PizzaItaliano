using Convey.HTTP;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
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
        private readonly IDistributedCache _distributedCache;
        private readonly RequestsOptions _requestsOptions;
        private readonly string _url;

        public ProductServiceClient(IHttpClient httpClient, HttpClientOptions httpClientOptions, IDistributedCache distributedCache,
            RequestsOptions requestsOptions)
        {
            _httpClient = httpClient;
            _distributedCache = distributedCache;
            _requestsOptions = requestsOptions;
            _url = httpClientOptions.Services["products"];
        }

        public async Task<ProductDto> GetAsync(string id)
        {
            var product = await _distributedCache.GetStringAsync(GetKey(id));

            return string.IsNullOrWhiteSpace(product) ? null : JsonConvert.DeserializeObject<ProductDto>(product);
        }

        public async Task<ProductDto> GetAsync(Guid id)
        {
            var idString = id.ToString();
            var product = await GetAsync(idString);

            if (product is null)
            {
                product = await _httpClient.GetAsync<ProductDto>($"{_url}/products/{id}");

                await _distributedCache.SetStringAsync(GetKey(idString),
                JsonConvert.SerializeObject(product),
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(_requestsOptions.ExpirySeconds)
                });

            }
            return product;
        }

        private static string GetKey(string id) => $"requests:{id}";
    }
}

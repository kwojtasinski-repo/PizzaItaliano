using Convey.HTTP;
using PizzaItaliano.Services.Payments.Application.DTO;
using PizzaItaliano.Services.Payments.Application.Services.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Infrastructure.Services.Clients
{
    internal sealed class OrderServiceClient : IOrderServiceClient
    {
        private readonly IHttpClient _httpClient;
        private readonly string _url;

        public OrderServiceClient(IHttpClient httpClient, HttpClientOptions httpClientOptions)
        {
            _httpClient = httpClient;
            _url = httpClientOptions.Services["orders"];
        }

        public async Task<OrderDto> GetAsync(Guid orderId)
        {
            var order = await _httpClient.GetAsync<OrderDto>($"{_url}/orders/{orderId}");
            return order;
        }
    }
}

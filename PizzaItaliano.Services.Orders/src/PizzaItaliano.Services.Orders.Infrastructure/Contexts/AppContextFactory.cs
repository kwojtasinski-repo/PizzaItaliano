﻿using Convey.MessageBrokers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PizzaItaliano.Services.Orders.Application;
using System.Text.Json;

namespace PizzaItaliano.Services.Orders.Infrastructure.Contexts
{
    internal sealed class AppContextFactory : IAppContextFactory
    {
        private readonly ICorrelationContextAccessor _contextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppContextFactory(ICorrelationContextAccessor contextAccessor, IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = contextAccessor;
            _httpContextAccessor = httpContextAccessor;
        }

        public IAppContext Create()
        {
            if (_contextAccessor.CorrelationContext is not null)
            {
                string payload;
                if (typeof(JsonElement).IsAssignableFrom(_contextAccessor.CorrelationContext.GetType()))
                {
                    payload = _contextAccessor.CorrelationContext.ToString();
                }
                else
                {
                    payload = JsonConvert.SerializeObject(_contextAccessor.CorrelationContext);
                }

                return string.IsNullOrWhiteSpace(payload)
                    ? AppContext.Empty
                    : new AppContext(JsonConvert.DeserializeObject<CorrelationContext>(payload));
            }

            var context = _httpContextAccessor.GetCorrelationContext();

            return context is null ? AppContext.Empty : new AppContext(context);
        }
    }
}

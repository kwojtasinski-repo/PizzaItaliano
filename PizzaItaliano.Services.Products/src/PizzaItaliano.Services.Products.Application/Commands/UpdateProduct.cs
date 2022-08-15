﻿using Convey.CQRS.Commands;
using System;

namespace PizzaItaliano.Services.Products.Application.Commands
{
    [Contract]
    public class UpdateProduct : ICommand
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public decimal? Cost { get; set; }

        public UpdateProduct(Guid productId, string name, decimal? cost)
        {
            ProductId = productId;
            Name = name;
            Cost = cost;
        }
    }
}

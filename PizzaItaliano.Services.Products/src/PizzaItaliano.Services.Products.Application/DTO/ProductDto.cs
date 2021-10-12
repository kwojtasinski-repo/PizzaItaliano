using System;

namespace PizzaItaliano.Services.Products.Application.DTO
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
    }
}

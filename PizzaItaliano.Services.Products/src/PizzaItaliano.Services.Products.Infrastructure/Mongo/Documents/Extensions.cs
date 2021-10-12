using PizzaItaliano.Services.Products.Application.DTO;
using PizzaItaliano.Services.Products.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Infrastructure.Mongo.Documents
{
    internal static class Extensions
    {
        public static Product AsEntity(this ProductDocument productDocument)
        {
            var entity = new Product(productDocument.Id, productDocument.Name, productDocument.Cost);
            return entity;
        }

        public static ProductDocument AsDocument(this Product entity)
        {
            var document = new ProductDocument
            {
                Id = entity.Id,
                Cost = entity.Cost,
                Name = entity.Name
            };
            return document;
        }

        public static ProductDto AsDto(this Product product)
        {
            var dto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Cost = product.Cost
            };
            return dto;
        }

        public static ProductDto AsDto(this ProductDocument productDocument)
        {
            var dto = new ProductDto
            {
                Id = productDocument.Id,
                Name = productDocument.Name,
                Cost = productDocument.Cost
            };
            return dto;
        }
    }
}

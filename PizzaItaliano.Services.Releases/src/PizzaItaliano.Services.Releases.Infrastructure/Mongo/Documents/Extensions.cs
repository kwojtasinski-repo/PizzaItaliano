using PizzaItaliano.Services.Releases.Application.DTO;
using PizzaItaliano.Services.Releases.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Infrastructure.Mongo.Documents
{
    internal static class Extensions
    {
        public static Release AsEntity(this ReleaseDocument document)
        {
            var release = new Release(document.Id, document.OrderId, document.OrderProductId, document.Date);
            return release;
        }

        public static ReleaseDocument AsDocument(this Release entity)
        {
            var releaseDocument = new ReleaseDocument
            {
                Id = entity.Id,
                Date = entity.Date,
                OrderId = entity.OrderId,
                OrderProductId = entity.OrderProductId
            };

            return releaseDocument;
        }

        public static ReleaseDto AsDto(this ReleaseDocument document)
        {
            var releaseDto = new ReleaseDto
            {
                Id = document.Id,
                OrderId = document.OrderId,
                Date = document.Date,
                OrderProductId = document.OrderProductId
            };

            return releaseDto;
        }
    }
}

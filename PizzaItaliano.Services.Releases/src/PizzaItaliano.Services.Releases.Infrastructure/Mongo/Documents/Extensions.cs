using PizzaItaliano.Services.Releases.Application.DTO;
using PizzaItaliano.Services.Releases.Core.Entities;

namespace PizzaItaliano.Services.Releases.Infrastructure.Mongo.Documents
{
    internal static class Extensions
    {
        public static Release AsEntity(this ReleaseDocument document)
        {
            var release = new Release(document.Id, document.OrderId, document.OrderProductId, document.Date, document.UserId);
            return release;
        }

        public static ReleaseDocument AsDocument(this Release entity)
        {
            var releaseDocument = new ReleaseDocument
            {
                Id = entity.Id,
                Date = entity.Date,
                OrderId = entity.OrderId,
                OrderProductId = entity.OrderProductId,
                UserId = entity.UserId
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
                OrderProductId = document.OrderProductId,
                UserId = document.UserId
            };

            return releaseDto;
        }
    }
}

using PizzaItaliano.Services.Identity.Application.DTO;
using PizzaItaliano.Services.Identity.Core.Entities;
using System.Linq;

namespace PizzaItaliano.Services.Identity.Infrastructure.Mongo.Documents
{
    internal static class Extensions
    {
        public static User AsEntity(this UserDocument document)
               => new(document.Id, document.Email, document.Password, document.Role, document.CreatedAt,
                   document.Permissions);

        public static UserDocument AsDocument(this User entity)
            => new()
            {
                Id = entity.Id,
                Email = entity.Email.Value,
                Password = entity.Password,
                Role = entity.Role,
                CreatedAt = entity.CreatedAt,
                Permissions = entity.Permissions ?? Enumerable.Empty<string>()
            };

        public static UserDto AsDto(this UserDocument document)
           => new()
           {
               Id = document.Id,
               Email = document.Email,
               Role = document.Role,
               CreatedAt = document.CreatedAt,
               Permissions = document.Permissions ?? Enumerable.Empty<string>()
           };

        public static RefreshToken AsEntity(this RefreshTokenDocument document)
            => new(document.Id, document.UserId, document.Token, document.CreatedAt, document.RevokedAt);

        public static RefreshTokenDocument AsDocument(this RefreshToken entity)
            => new()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Token = entity.Token,
                CreatedAt = entity.CreatedAt,
                RevokedAt = entity.RevokedAt
            };
    }
}

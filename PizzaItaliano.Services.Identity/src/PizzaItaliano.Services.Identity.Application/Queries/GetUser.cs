using Convey.CQRS.Queries;
using PizzaItaliano.Services.Identity.Application.DTO;
using System;

namespace PizzaItaliano.Services.Identity.Application.Queries
{
    public class GetUser : IQuery<UserDto>
    {
        public Guid UserId { get; set; }
    }
}
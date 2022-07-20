using Convey.CQRS.Commands;
using System;

namespace PizzaItaliano.Services.Identity.Application.Commands
{
    public class ChangeUserRole : ICommand
    {
        public Guid Id { get; set; }
        public string Role { get; set; }

        public ChangeUserRole(Guid id, string role)
        {
            Id = id;
            Role = role;
        }
    }
}

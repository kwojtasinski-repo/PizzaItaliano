using System;

namespace PizzaItaliano.Services.Orders.Application.Exceptions
{
    public class InvalidUserIdException : AppException
    {
        public override string Code => "invalid_user_id";
        public Guid UserId { get; }

        public InvalidUserIdException(Guid userId) : base($"User has invalid id: '{userId}'")
        {
            UserId = userId;
        }
    }
}

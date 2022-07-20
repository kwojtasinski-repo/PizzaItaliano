namespace PizzaItaliano.Services.Identity.Application.Exceptions
{
    public class InvalidUserIdException : AppException
    {
        public override string Code { get; } = "invalid_user_id";

        public InvalidUserIdException() : base("Invalid UserId")
        {
        }
    }
}

namespace PizzaItaliano.Services.Identity.Core.Exceptions
{
    public class InvalidPasswordException : DomainException
    {
        public override string Code { get; } = "invalid_password";

        public InvalidPasswordException() : base($"Invalid password. Password should have at least 8 characters, including upper letter and number")
        {
        }
    }
}

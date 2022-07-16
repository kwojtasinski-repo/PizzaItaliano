namespace PizzaItaliano.Services.Identity.Application.Exceptions
{
    public class PasswordsAreNotSameException : AppException
    {
        public override string Code { get; } = "passwords_are_not_same";

        public PasswordsAreNotSameException() : base("Passwords are not same.")
        {
        }
    }
}

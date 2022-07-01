namespace PizzaItaliano.Services.Identity.Application.Exceptions
{
    public class InvalidRefreshTokenException : AppException
    {
        public override string Code { get; } = "invalid_refresh_token";

        public InvalidRefreshTokenException() : base("Invalid refresh token.")
        {
        }
    }
}

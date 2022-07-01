namespace PizzaItaliano.Services.Identity.Application.Exceptions
{
    public class RevokedRefreshTokenException : AppException
    {
        public override string Code { get; } = "revoked_refresh_token";

        public RevokedRefreshTokenException() : base("Revoked refresh token.")
        {
        }
    }
}
namespace PizzaItaliano.Services.Orders.Application.Exceptions
{
    public class UnauthorizedException : AppException
    {
        public override string Code { get; } = "unauthorized";

        public UnauthorizedException() : base($"Unauthorized")
        {
        }
    }
}

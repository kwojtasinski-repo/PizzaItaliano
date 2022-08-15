namespace PizzaItaliano.Services.Orders.Application.Exceptions
{
    public class InvalidOrderIdException : AppException
    {
        public override string Code { get; } = "invalid_order_id";

        public InvalidOrderIdException() : base("Invalid OrderId")
        {
        }
    }
}

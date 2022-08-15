namespace PizzaItaliano.Services.Products.Application.Exceptions
{
    public class InvalidProductIdException : AppException
    {
        public override string Code => "invalid_product_id";

        public InvalidProductIdException() : base($"Invalid ProductId")
        {
        }
    }
}

namespace PizzaItaliano.Services.Identity.Application.Services
{
    internal interface IPasswordService
    {
        bool IsValid(string hash, string password);
        string Hash(string password);
    }
}
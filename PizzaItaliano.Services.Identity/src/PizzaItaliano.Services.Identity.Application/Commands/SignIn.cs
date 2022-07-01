using Convey.CQRS.Commands;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Application.Commands
{
    [Contract]
    public class SignIn : ICommand
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public SignIn(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }

    internal class SignInHandler : ICommandHandler<SignIn>
    {
        public Task HandleAsync(SignIn command)
        {
            return Task.CompletedTask;
        }
    }
}
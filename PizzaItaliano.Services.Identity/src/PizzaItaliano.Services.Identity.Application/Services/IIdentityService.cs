using PizzaItaliano.Services.Identity.Application.Commands;
using PizzaItaliano.Services.Identity.Application.DTO;
using System;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Application.Services
{
    public interface IIdentityService
    {
        Task<UserDto> GetAsync(Guid id);
        Task<AuthDto> SignInAsync(SignIn command);
        Task SignUpAsync(SignUp command);
    }
}

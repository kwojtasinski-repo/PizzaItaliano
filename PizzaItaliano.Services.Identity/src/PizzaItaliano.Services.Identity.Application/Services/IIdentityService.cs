using PizzaItaliano.Services.Identity.Application.Commands;
using PizzaItaliano.Services.Identity.Application.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Application.Services
{
    public interface IIdentityService
    {
        Task<UserDto> GetAsync(Guid id);
        Task<IList<UserDto>> GetAllAsync();
        Task<AuthDto> SignInAsync(SignIn command);
        Task SignUpAsync(SignUp command);
        Task ChangePasswordAsync(ChangePassword changePassword);
    }
}

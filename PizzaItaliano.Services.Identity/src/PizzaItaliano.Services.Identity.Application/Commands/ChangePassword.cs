using Convey.CQRS.Commands;

namespace PizzaItaliano.Services.Identity.Application.Commands
{
    public class ChangePassword : ICommand
    {
        public string Email { get; }
        public string OldPassword { get; }
        public string NewPassword { get; }
        public string NewPasswordConfirm { get; }

        public ChangePassword(string email, string oldPassword, string newPassword, string newPasswordConfirm)
        {
            Email = email;
            OldPassword = oldPassword;
            NewPassword = newPassword;
            NewPasswordConfirm = newPasswordConfirm;
        }
    }
}

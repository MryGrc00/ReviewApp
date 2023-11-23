using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        LoginResult AuthenticateUser(string email, string password, ref Admin admin);
        void AddUser(UserViewModel model);
        void GetCode(ForgotPasswordViewModel model);
        ForgotPasswordViewModel ForgotPassword(ForgotPasswordViewModel model);
        void ResetPassword(AdminViewModel model);
    }
}

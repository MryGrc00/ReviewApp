using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.IO;
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IForgotPasswordRepository _forgotPasswordRepository;
        private readonly IAdminService _adminService;
        private readonly IAdminRepository _adminRepository;

        public UserService(IUserRepository repository, IEmailSender emailSender, IForgotPasswordRepository forgotPasswordRepository, 
            IAdminService adminService, IAdminRepository adminRepository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
            _emailSender = emailSender;
            _forgotPasswordRepository = forgotPasswordRepository;
            _adminService = adminService;
            _adminRepository = adminRepository;
        }

        /// <summary>
        /// Verifies user login details against stored credentials.
        /// </summary>
        /// <param name="userId">Email of the user attempting to log in.</param>
        /// <param name="password">Password for login attempt.</param>
        /// <param name="admin">Reference to store logged-in user's details.</param>
        /// <returns>Status of login attempt: success or failure.</returns>
        public LoginResult AuthenticateUser(string userId, string password, ref Admin admin)
        {
            admin = new Admin();
            var passwordKey = PasswordManager.EncryptPassword(password);
            admin = _repository.GetUsers().Where(x => x.Email == userId &&
                                                     x.Password == passwordKey).FirstOrDefault();

            return admin != null ? LoginResult.Success : LoginResult.Failed;
        }

        /// <summary>
        /// Creates a new user account, ensuring the email is unique.
        /// </summary>
        /// <param name="model">Details of the user to be added.</param>
        public void AddUser(UserViewModel model)
        {
            var admin = new Admin();
            if (!_repository.UserExists(model.Email))
            {
                _mapper.Map(model, admin);
                admin.Password = PasswordManager.EncryptPassword(model.Password);
                admin.CreatedTime = DateTime.Now;
                admin.UpdatedTime = DateTime.Now;
                admin.CreatedBy = System.Environment.UserName;
                admin.UpdatedBy = System.Environment.UserName;

                _repository.AddUser(admin);
            }
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            }
        }

        /// <summary>
        /// Generates and sends a password reset code to a user's email.
        /// </summary>
        /// <param name="model">User's email for sending reset code.</param>
        public void GetCode(ForgotPasswordViewModel model)
        {
            var data = _adminService.GetAdminWithEmail(model.Email);
            if (data != null)
            {
                ForgotPassword forgetPassword = new ForgotPassword();
                forgetPassword.Email = data.Email;
                forgetPassword.Code = Guid.NewGuid().ToString("N").Substring(0, 6);
                forgetPassword.DateReset = DateTime.Now;
                var message = new Message(new string[] { forgetPassword.Email }, "Forgot Password", $"This is your code {forgetPassword.Code}.");
                _emailSender.SendEmail(message);

                _forgotPasswordRepository.AddForgotPassword(forgetPassword);
            }
        }

        /// <summary>
        /// Verifies the password reset code entered by the user.
        /// </summary>
        /// <param name="model">Contains user's email and the reset code.</param>
        /// <returns>Reset status based on code verification.</returns>
        public ForgotPasswordViewModel ForgotPassword (ForgotPasswordViewModel model)
        {
            var data = _forgotPasswordRepository.GetForgetPassword(model.Email);
            if (data != null)
            {
                if (data.Code == model.Code)
                {
                    ForgotPasswordViewModel reset = new()
                    {
                        Email = data.Email,
                    };
                    var remove = _forgotPasswordRepository.GetForgetPassword(reset.Email);
                    _forgotPasswordRepository.DeleteForgotPassword(remove);
                    return reset;
                }
                ForgotPasswordViewModel email = new()
                {
                    Email = data.Email,
                };

                return email;
            }
            return null;
        }

        /// <summary>
        /// Resets the password for a user's account.
        /// </summary>
        /// <param name="model">Contains user's email and new password.</param>
        public void ResetPassword (AdminViewModel model)
        {
            var data = _adminRepository.GetAdminByEmail(model.Email);
            if (data != null)
            {
                data.Password = PasswordManager.EncryptPassword(model.Password);
                data.UpdatedTime = DateTime.Now;
                _adminRepository.EditAdmin(data);
            }
        }

    }
}

using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.ReviewAppAdmin.Authentication;
using ASI.Basecode.ReviewAppAdmin.Models;
using ASI.Basecode.ReviewAppAdmin.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.ReviewAppAdmin.Controllers
{
    public class AccountController : ControllerBase<AccountController>
    {
        private readonly SessionManager _sessionManager;
        private readonly SignInManager _signInManager;
        private readonly TokenValidationParametersFactory _tokenValidationParametersFactory;
        private readonly TokenProviderOptionsFactory _tokenProviderOptionsFactory;
        private readonly IConfiguration _appConfiguration;
        private readonly IUserService _userService;
        private readonly IAdminService _adminService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="localizer">The localizer.</param>
        /// <param name="userService">The admin service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
        /// <param name="tokenProviderOptionsFactory">The token provider options factory.</param>
        public AccountController(
                            IAdminService adminService,
                            SignInManager signInManager,
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper,
                            IUserService userService,
                            TokenValidationParametersFactory tokenValidationParametersFactory,
                            TokenProviderOptionsFactory tokenProviderOptionsFactory) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._sessionManager = new SessionManager(this._session);
            this._signInManager = signInManager;
            this._tokenProviderOptionsFactory = tokenProviderOptionsFactory;
            this._tokenValidationParametersFactory = tokenValidationParametersFactory;
            this._appConfiguration = configuration;
            this._userService = userService;
            this._adminService = adminService;
        }

        /// <summary>
        /// Displays the login view. Initializes a new session.
        /// </summary>
        /// <returns>Login view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            TempData["returnUrl"] = System.Net.WebUtility.UrlDecode(HttpContext.Request.Query["ReturnUrl"]);
            this._sessionManager.Clear();
            this._session.SetString("SessionId", System.Guid.NewGuid().ToString());
            return this.View();
        }

        /// <summary>
        /// Authenticates and signs in an admin based on provided credentials.
        /// </summary>
        /// <param name="model">Contains admin's login credentials.</param>
        /// <param name="returnUrl">URL to return to after successful login.</param>
        /// <returns>Redirects to the dashboard on successful login, or shows login view with error message.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            this._session.SetString("HasSession", "Exist");

            Admin admin = null;
            var loginResult = _userService.AuthenticateUser(model.Email, model.Password, ref admin);
            if (loginResult == LoginResult.Success)
            {
                // 認証OK
                await this._signInManager.SignInAsync(admin);
                this._session.SetString("UserName", admin.Name);
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                // 認証NG
                TempData["ErrorMessage"] = "Incorrect Email or Password";
                return View();
            }
            return View();
        }

        /// <summary> Hi
        /// Displays the forgot password view.
        /// </summary>
        /// <returns>Forgot Password view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Processes the request for a password reset code based on the provided email.
        /// </summary>
        /// <param name="forgotPassword">Contains the email for password reset.</param>
        /// <returns>Forgot Password view with the result of the request.</returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult GetCode(ForgotPasswordViewModel forgotPassword)
        {
            var data = _userService.ForgotPassword(forgotPassword);
            _userService.GetCode(forgotPassword);
            return View("ForgotPassword", data);
        }

        ///<summary>
        /// Processes the forgot password request and initiates password reset process.
        /// </summary>
        /// <param name="forgotPassword">Contains the email for password reset.</param>
        /// <returns>Redirects to Reset Password view or returns with an error message.</returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ForgotPassword(ForgotPasswordViewModel forgotPassword)
        {
            var checkEmail = _adminService.CheckEmail(forgotPassword.Email);
            if (checkEmail != "Exist")
            {
                base.ModelState.AddModelError("Email", "Email doesn't exists.");
                return View(forgotPassword);
            }
            if (checkEmail == "Invalid")
            {
                base.ModelState.AddModelError("Email", "Email does not contain @gmail.com");
                return View(forgotPassword);
            }
            
            _userService.ForgotPassword(forgotPassword);

            return RedirectToAction("ResetPassword", new { email = forgotPassword.Email});
        }

        ///<summary>
        /// Displays the reset password view for a specific email.
        /// </summary>
        /// <param name="email">Email address for resetting the password.</param>
        /// <returns>Reset Password view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email)
        {
            var data = _adminService.GetAdminWithEmail(email);
            return View(data);
        }


        /// <summary>
        /// Processes the password reset request.
        /// </summary>
        /// <param name="admin">Admin view model containing new password record.</param>
        /// <returns>Redirects to the Login view upon successful reset or returns to the same view with error message.</returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ResetPassword(AdminViewModel admin)
        {
            var checkPassword = _adminService.CheckPasswords(admin.Password, admin.ConfirmPassword);
            if (checkPassword == "PassInvalid")
            {
                base.ModelState.AddModelError("Password", "Password must have at least one special character, one lowercase letter, one uppercase letter, and one digit.");
                return View(admin);
            }
            if (checkPassword == "PassShort")
            {
                base.ModelState.AddModelError("Password", "Password must at least 8 characters.");
                return View(admin);
            }
            if (checkPassword == "ConInvalid")
            {
                base.ModelState.AddModelError("ConfirmPassword", "Confirm Password must have at least one special character, one lowercase letter, one uppercase letter, and one digit.");
                return View(admin);
            }
            if (checkPassword == "ConShort")
            {
                base.ModelState.AddModelError("ConfirmPassword", "Confirm Password must at least 8 characters.");
                return View(admin);
            }
            if (checkPassword == "NotMatch")
            {
                base.ModelState.AddModelError("Password", "Passwords do not match.");
                base.ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                return View(admin);
            }
            _userService.ResetPassword(admin);
            return RedirectToAction("Login", "Account");
        }

        ///<summary>
        /// Displays the user registration view.
        /// </summary>
        /// <returns>Registration view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Processes user registration.
        /// </summary>
        /// <param name="model">User view model containing registration records.</param>
        /// <returns>Redirects to the Login view upon successful registration or returns to the same view with error message.</returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(UserViewModel model)
        {
            try
            {
                _userService.AddUser(model);
                return RedirectToAction("Login", "Account");
            }
            catch(InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;
            }
            return View();
        }

        /// <summary>
        /// Signs out the current user and redirects to the login view.
        /// </summary>
        /// <returns>Redirects to the Login view after signing out.</returns>
        [AllowAnonymous]
        public async Task<IActionResult> SignOutUser()
        {
            await this._signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}

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
        /// Login Method
        /// </summary>
        /// <returns>Created response view</returns>
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
        /// Authenticate admin and signs the admin in when successful.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns> Created response view </returns>
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult GetCode(ForgotPasswordViewModel forgotPassword)
        {
            var data = _userService.ForgotPassword(forgotPassword);
            _userService.GetCode(forgotPassword);
            return View("ForgotPassword", data);
        }

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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email)
        {
            var data = _adminService.GetAdminWithEmail(email);
            return View(data);
        }
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

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
        /// Sign Out current account and return login view.
        /// </summary>
        /// <returns>Created response view</returns>
        [AllowAnonymous]
        public async Task<IActionResult> SignOutUser()
        {
            await this._signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}

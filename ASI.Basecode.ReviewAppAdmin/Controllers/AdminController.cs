﻿using ASI.Basecode.Data.Models;
using ASI.Basecode.ReviewAppAdmin.Mvc;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.ReviewAppAdmin.Controllers
{
    public class AdminController : ControllerBase<AdminController>
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IConfiguration configuration, IMapper mapper = null)
            : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// List of Admin Records
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult List(int page = 1, int pageSize = 5)
        {
            var admins = _adminService.PaginatedAdmins(page, pageSize);
            return View("List", admins);
        }

        /// <summary>
        /// Add Method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddAdmin()
        {
            return View();
        }

        /// <summary>
        /// Add admin record to the database
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddAdmin(AdminViewModel admin)
        {
            var checkEmail = _adminService.CheckEmail(admin.Email);
            if (checkEmail == "Exist")
            {
                base.ModelState.AddModelError("Email", "Email already exists.");
                return View(admin);
            }
            if (checkEmail == "Invalid")
            {
                base.ModelState.AddModelError("Email", "Email does not contain @gmail.com");
                return View(admin);
            }
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
            _adminService.AddAdmin(admin, this.UserName);
            return RedirectToAction("List");
        }

        /// <summary>
        /// Edit Method
        /// </summary>
        /// <param name="AdminId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditAdmin(int AdminId)
        {
            var data = _adminService.GetAdminWithPassword(AdminId);
            return View(data);
        }

        /// <summary>
        /// Update the admin record to the database
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EditAdmin(AdminViewModel admin)
        {
            _adminService.UpdateAdmin(admin, this.UserName);
            return RedirectToAction("List");
        }

        /// <summary>
        /// Delete admin record to the database
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteAdmin(AdminViewModel admin)
        {
            _adminService.DeleteAdmin(admin);
            return RedirectToAction("List");
        }

        /// <summary>
        /// View admin record
        /// </summary>
        /// <param name="AdminId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ViewAdmin(int AdminId)
        {
            var data = _adminService.GetAdmin(AdminId);
            return View(data);
        }
    }
}

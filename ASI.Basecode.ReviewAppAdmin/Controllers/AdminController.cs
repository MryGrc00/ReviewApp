﻿using ASI.Basecode.Data.Models;
using ASI.Basecode.ReviewAppAdmin.Mvc;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        ///<summary>
        /// Retrieves and displays a paginated list of admin records.
        /// </summary>
        /// <param name="page">Page number to display. Default is 1.</param>
        /// <param name="pageSize">Number of records per page. Default is 5.</param>
        /// <returns>View with the list of admin records.</returns>
        [HttpGet]
        public IActionResult List(int page = 1, int pageSize = 5)
        {
            var admins = _adminService.PaginatedAdmins(page, pageSize);
            return View("List", admins);
        }

        /// <summary>
        /// Displays the view for adding a new admin record.
        /// </summary>
        /// <returns>Add Admin view.</returns>
        [HttpGet]
        public IActionResult AddAdmin()
        {
            return View();
        }

        /// <summary>
        /// Processes the addition of a new admin record after validating required fields.
        /// </summary>
        /// <param name="admin">Admin record to add.</param>
        /// <returns>Redirects to the admin list on success, or displays validation errors.</returns>
        [HttpPost]
        public IActionResult AddAdmin(AdminViewModel admin)
        {
            if (admin.Email == null || admin.Email == " ")
            {
                base.ModelState.AddModelError("Email", "Email is required");
                return View(admin);
            }
            if (admin.Name == null || admin.Name == " ")
            {
                base.ModelState.AddModelError("Name", "Name is required");
                return View(admin);
            }
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
            TempData["SuccessMessage"] = "Admin successfully added.";
            return RedirectToAction("List");
        }

        /// <summary>
        /// Fetches and displays the records of an admin for editing.
        /// </summary>
        /// <param name="AdminId">ID of the admin to edit.</param>
        /// <returns>Edit Admin view populated with admin's records.</returns>
        [HttpGet]
        public IActionResult EditAdmin(int AdminId)
        {
            var data = _adminService.GetAdminWithPassword(AdminId);
            return View(data);
        }

        /// <summary>
        /// Updates an admin record with new record.
        /// </summary>
        /// <param name="admin">Updated admin record.</param>
        /// <returns>Redirects to the admin list after successful update.</returns>
        [HttpPost]
        public IActionResult EditAdmin(AdminViewModel admin)
        {
            _adminService.UpdateAdmin(admin, this.UserName);
            TempData["SuccessMessage"] = "Admin successfully updated.";
            return RedirectToAction("List");
        }

        /// <summary>
        /// Deletes an admin record from the system.
        /// </summary>
        /// <param name="admin">Admin record to delete.</param>
        /// <returns>Redirects to the admin list after deletion.</returns>
        [HttpPost]
        public IActionResult DeleteAdmin(AdminViewModel admin)
        {
            _adminService.DeleteAdmin(admin);
            TempData["SuccessMessage"] = "Admin successfully deleted.";
            return RedirectToAction("List");
        }

        /// <summary>
        /// Displays detailed information of a specific admin.
        /// </summary>
        /// <param name="AdminId">ID of the admin to view.</param>
        /// <returns>View Admin view with the admin's details.</returns>
        [HttpGet]
        public IActionResult ViewAdmin(int AdminId)
        {
            var data = _adminService.GetAdminWithPassword(AdminId);
            return View(data);
        }

        /// <summary>
        /// Displays the account settings view for the current admin.
        /// </summary>
        /// <returns>Account Settings view.</returns>
        [HttpGet]
        public IActionResult AccountSetting()
        {
            return View();
        }

        /// <summary>
        /// Displays the view for changing the current admin's email.
        /// </summary>
        /// <returns>Change Email view with current admin's details.</returns>
        [HttpGet]
        public IActionResult ChangeEmail ()
        {
            var data = _adminService.GetAdminWithName(this.UserName);
            return View(data);
        }

        /// <summary>
        /// Processes the request to change the current admin's email.
        /// </summary>
        /// <param name="admin">Admin record with the new email.</param>
        /// <returns>Redirects to the dashboard after successful change.</returns>
        [HttpPost]
        public IActionResult ChangeEmail(AdminViewModel admin)
        {
            _adminService.ChangeEmail(admin, this.UserName);
            TempData["SuccessMessage"] = "Admin successfully changed email.";
            return RedirectToAction("Index", "Dashboard");
        }

        /// <summary>
        /// Displays the view for changing the current admin's name.
        /// </summary>
        /// <returns>Change Name view with current admin's details.</returns>
        [HttpGet]
        public IActionResult ChangeName ()
        {
            var data = _adminService.GetAdminWithName(this.UserName);
            return View(data);
        }

        /// <summary>
        /// Updates the name of the currently logged-in admin in the database.
        /// </summary>
        /// <param name="admin">The admin view model containing the new name information.</param>
        /// <returns>Redirects to the Dashboard Index view on successful name update.</returns>
        [HttpPost]
        public IActionResult ChangeName(AdminViewModel admin)
        {
            _adminService.ChangeName(admin, this.UserName);
            this._session.SetString("UserName", admin.Name);
            TempData["SuccessMessage"] = "Admin successfully changed name.";
            return RedirectToAction("Index", "Dashboard");
        }

        /// <summary>
        /// Displays the view for changing the current admin's password.
        /// </summary>
        /// <returns>Change Password view with current admin's details.</returns>
        [HttpGet]
        public IActionResult ChangePassword()
        {
            var data = _adminService.GetAdminWithName(this.UserName);
            return View(data);
        }

        /// <summary>
        /// Processes the request to change the current admin's password after validating criteria.
        /// </summary>
        /// <param name="admin">The admin view model containing the old and new password record.</param>
        /// <returns>Returns to the Change Password view with validation messages on failure or redirects to the Dashboard Index view on successful password update.</returns>
        [HttpPost]
        public IActionResult ChangePassword(AdminViewModel admin)
        {
            var oldPassword = _adminService.CheckOldPassword(admin.AdminId, admin.oldPassword);
            if(oldPassword == "Not Match")
            {
                base.ModelState.AddModelError("oldPassword", "Password do not match");
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
            _adminService.ChangePassword(admin, this.UserName);
            TempData["SuccessMessage"] = "Admin successfully changed password.";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}

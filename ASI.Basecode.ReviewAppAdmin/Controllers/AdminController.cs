using ASI.Basecode.Data.Models;
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

        [HttpGet]
        public IActionResult List(int page = 1, int pageSize = 5)
        {
            List<AdminViewModel> data = _adminService.GetAdmins();

            int totalCount = data.Count;
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));

            List<AdminViewModel> paginatedAdmins = data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalPages"] = totalPages;
            return View("List", paginatedAdmins);
        }

        [HttpGet]
        public IActionResult AddAdmin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddAdmin(AdminViewModel admin)
        {
            var isExistEmail = _adminService.CheckEmail(admin.Email);
            if (isExistEmail)
            {
                base.ModelState.AddModelError("Email", "Isbn already exists.");
                return View(admin);
            }
            _adminService.AddAdmin(admin, this.UserName);
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult EditAdmin(int AdminId)
        {
            var data = _adminService.GetAdmin(AdminId);
            return View(data);
        }

        [HttpPost]
        public IActionResult EditAdmin(AdminViewModel admin)
        {
            _adminService.UpdateAdmin(admin, this.UserName);
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult DeleteAdmin(AdminViewModel admin)
        {
            _adminService.DeleteAdmin(admin);
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult ViewAdmin(int AdminId)
        {
            var data = _adminService.GetAdmin(AdminId);
            return View(data);
        }
    }
}

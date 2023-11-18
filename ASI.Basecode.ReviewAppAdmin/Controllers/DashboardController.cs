using ASI.Basecode.ReviewAppAdmin.Mvc;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.ReviewAppAdmin.Controllers
{
    public class DashboardController : ControllerBase<DashboardController>
    {
        private readonly IBookService _bookService;

        public  DashboardController (IBookService bookService, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IConfiguration configuration, IMapper mapper = null)
            : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<BookViewModel> newest = _bookService.GetBooks().Take(5).ToList();
            List<BookViewModel> topBooks = _bookService.TopBooks().Take(5).ToList();
            ViewData["TopBooks"] = topBooks;
            return View("Index", newest);
        }

        [HttpGet]
        public IActionResult ViewBookAndReview(int BookId)
        {
            var data = _bookService.GetBook(BookId);
            return View(data);
        }
    }
}

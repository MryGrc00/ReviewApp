using System;
using System.Collections.Generic;
using System.Linq;
using ASI.Basecode.Data.Models;
using ASI.Basecode.ReviewAppAdmin.Mvc;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ASI.Basecode.ReviewAppAdmin.Controllers
{
    public class BookController : ControllerBase<BookController>
    {
        private readonly IBookService _bookService;
        private readonly IGenreService _genreService;

        public BookController(IBookService bookService, IGenreService genreService, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IConfiguration configuration, IMapper mapper = null)
            : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookService = bookService;
            _genreService = genreService;
        }

        [HttpGet]
        public IActionResult List(int page = 1, int pageSize = 5)
        {
            List<BookViewModel> data = _bookService.GetBooks();

            int totalCount = data.Count;
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));

            List<BookViewModel> paginatedBooks = data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalPages"] = totalPages;

            return View("List", paginatedBooks);
        }

        [HttpGet]
        public IActionResult AddBook()
        {
            List<GenreViewModel> data = _genreService.GetGenres();
            ViewData["Genre"] = data;
            return View("AddBook", new ASI.Basecode.Services.ServiceModels.BookViewModel());
        }

        [HttpPost]
        public IActionResult AddBook (BookViewModel book)
        {
            var isExist = _bookService.CheckIsbn(book.Isbn);
            if(isExist)
            {
                base.ModelState.AddModelError("Isbn", "Isbn already exists.");
                return View(book);
            }
            var isExistTitle = _bookService.CheckTitle(book.Title);
            if(isExistTitle)
            {
                base.ModelState.AddModelError("Title", "Title already exisits");
                return View(book);
            }
            _bookService.AddBook(book, this.UserName);
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult EditBook(int BookId)
        {
            var data = _bookService.GetBook(BookId);
            List<GenreViewModel> GenreData = _genreService.GetGenres();
            ViewData["Genre"] = GenreData;
            return View(data);
        }

        [HttpPost]
        public IActionResult EditBook(BookViewModel book)
        {
            _bookService.UpdateBook(book, this.UserName);
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult DeleteBook(BookViewModel book)
        {
            _bookService.DeleteBook(book);
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult ViewBook(int BookId)
        {
            var data = _bookService.GetBook(BookId);
            return View(data);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        /// <summary>
        /// List of Book Records
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult List(int page = 1, int pageSize = 5)
        {
            var books = _bookService.ListBooks(page, pageSize);
            return View("List", books);
        }

        /// <summary>
        /// Add Method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddBook()
        {
            List<GenreViewModel> data = _genreService.GetGenres();
            ViewData["Genre"] = data;
            return View("AddBook");
        }

        /// <summary>
        /// Add book record to the database
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddBook (BookViewModel book)
        {
            if (book.Title == null || book.Title == " ")
            {
                base.ModelState.AddModelError("Title", "Title is required");
                return View(book);
            }
            if (book.Description == null || book.Description == " ")
            {
                base.ModelState.AddModelError("Description", "Description is required");
                return View(book);
            }
            if (book.Isbn == null || book.Isbn == " ")
            {
                base.ModelState.AddModelError("Isbn", "Isbn is required");
                return View(book);
            }
            if (book.Author == null || book.Author == " ")
            {
                base.ModelState.AddModelError("Author", "Author is required");
                return View(book);
            }
            if (book.Genre == null || book.Genre == " ")
            {
                base.ModelState.AddModelError("Genre", "Other Fields is empty.");
                List<GenreViewModel> data = _genreService.GetGenres();
                ViewData["Genre"] = data;
                return View(book);
            }
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
            TempData["SuccessMessage"] = "Book successfully added.";
            return RedirectToAction("List");
        }

        /// <summary>
        /// Edit Method
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditBook(int BookId)
        {
            var data = _bookService.GetBook(BookId);
            List<GenreViewModel> GenreData = _genreService.GetGenres();
            ViewData["Genre"] = GenreData;
            return View(data);
        }

        /// <summary>
        /// Update book record to the database
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EditBook(BookViewModel book)
        {
            var genreChecker = _bookService.CheckGenre(book.Genre);
            if(genreChecker == false)
            {
                var data = _bookService.GetBook(book.BookId);
                base.ModelState.AddModelError("Genre", "Genre can't be null.");
                List<GenreViewModel> GenreData = _genreService.GetGenres();
                ViewData["Genre"] = GenreData;
                return View(data);
            }
            _bookService.UpdateBook(book, this.UserName);
            TempData["SuccessMessage"] = "Book successfully updated.";
            return RedirectToAction("List");
        }

        /// <summary>
        /// Delete book record to the database
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteBook(BookViewModel book)
        {
            _bookService.DeleteBook(book);
            TempData["SuccessMessage"] = "Book successfully deleted.";
            return RedirectToAction("List");
        }

        /// <summary>
        /// View book record and revoew associated
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ViewBook(int BookId, int page = 1, int pageSize = 5)
        {
            var data = _bookService.GetBook(BookId);
            var ratings = _bookService.ViewRatinginBooks(BookId, page, pageSize);
            ViewData["Ratings"] = ratings;
            return View("ViewBook", data);
        }
    }
}

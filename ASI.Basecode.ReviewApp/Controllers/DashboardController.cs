using ASI.Basecode.Data.Models;
using ASI.Basecode.ReviewApp.Mvc;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace ASI.Basecode.ReviewApp.Controllers
{
    public class DashboardController : ControllerBase<DashboardController>
    {
        private readonly IBookService _bookService;
        private readonly IRatingService _ratingService;
        private readonly IGenreService _genreService;

        public DashboardController(IBookService bookService, IRatingService ratingService, IGenreService genreService, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IConfiguration configuration, IMapper mapper = null)
            : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookService = bookService;
            _ratingService = ratingService;
            _genreService = genreService;
        }


        [HttpGet]
        public IActionResult Index()
        {
            List<BookViewModel> newest = _bookService.NewestBooksUser();
            List<BookViewModel> topBooks = _bookService.TopBooks();
            ViewData["TopBooks"] = topBooks;
            return View("Index", newest);
        }

        [HttpGet]
        public IActionResult NewestBookExpanded(int page = 1, int pageSize = 10, string searchKeyword = "", string sortBy = "")
        {
            var newestBookExpanded = _bookService.NewestBooksExpanded(page, pageSize, searchKeyword, sortBy);
            ViewBag.SearchKeyword = searchKeyword;
            ViewBag.SortBy = sortBy;
            return View("NewestBookExpanded", newestBookExpanded);
        }

        [HttpGet]
        public IActionResult TopBookExpanded(int page = 1, int pageSize = 10, string searchKeyword = "", string sortBy = "")
        {
            var topBookExpanded = _bookService.TopBooksExpanded(page, pageSize, searchKeyword, sortBy);

            return View("TopBookExpanded", topBookExpanded);
        }

        [HttpGet]
        public IActionResult ViewBookAndReview(int BookId)
        {
            var data = _bookService.GetBook(BookId);
            List<RatingViewModel> rating = _ratingService.GetRatings()
                .Where(x => x.BookId == BookId)
                .ToList();
            int starRating = _ratingService.GetRatings().Where(x => x.BookId == BookId).Sum(x => x.RateStars);
            ViewData["TotalStar"] = starRating;
            ViewData["Rate"] = rating;
            return View(data);
        }

        [HttpGet]
        public IActionResult RateBook(int BookId)
        {
            var data = _bookService.GetBook(BookId);
            int starRating = _ratingService.GetRatings().Where(x => x.BookId == BookId).Sum(x => x.RateStars);
            ViewData["TotalStar"] = starRating;
            ViewData["Book"] = data;
            return View();
        }

        [HttpPost]
        public IActionResult RateBook(RatingViewModel rating)
        {
            if (!rating.Email.Contains("@gmail.com"))
            {
                var book = _bookService.GetBook(rating.BookId);
                base.ModelState.AddModelError("Email", "Email is invalid.");
                ViewData["Book"] = book;
                return View(rating);
            }
            _ratingService.AddRating(rating);
            var data = _bookService.GetBook(rating.BookId);
            List<RatingViewModel> rate = _ratingService.GetRatings().Where(x => x.BookId == rating.BookId).ToList();
            ViewData["Rate"] = rate;
            TempData["SuccessMessage"] = "Rate successfully.";
            return RedirectToAction("ViewBookAndReview", data);
        }

        [HttpGet]
        public IActionResult GenreList(string genreName, int page = 1, int pageSize = 5)
        {
            var data = _genreService.GetGenreName(genreName);
            if (data != null)
            {
                var books = _genreService.ViewGenreInBooks(data.GenreName, page, pageSize);
                ViewData["Books"] = books;
                return View("GenreList", data);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}

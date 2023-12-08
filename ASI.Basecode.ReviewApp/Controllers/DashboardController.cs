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

        /// Retrieves and displays lists of the newest books and top rated books.
        /// </summary>
        /// <returns>Dashboard Index view with lists of newest and top rated books.</returns>
        [HttpGet]
        public IActionResult Index()
        {
            List<BookViewModel> newest = _bookService.NewestBooksUser();
            List<BookViewModel> topBooks = _bookService.TopBooks();
            ViewData["TopBooks"] = topBooks;
            return View("Index", newest);
        }

        /// <summary>
        /// Displays an expanded view of the newest books with options for pagination, search, and sorting.
        /// </summary>
        /// <param name="page">Current page number for pagination. Default is 1.</param>
        /// <param name="pageSize">Number of books per page. Default is 10.</param>
        /// <param name="searchKeyword">Search keyword for filtering books. Default is empty.</param>
        /// <param name="sortBy">Sorting criteria. Default is empty.</param>
        /// <returns>Newest Book Expanded view with filtered and sorted book listings.</returns>
        [HttpGet]
        public IActionResult NewestBookExpanded(int page = 1, int pageSize = 10, string searchKeyword = "", string sortBy = "")
        {
            var newestBookExpanded = _bookService.NewestBooksExpanded(page, pageSize, searchKeyword, sortBy);
            ViewBag.SearchKeyword = searchKeyword;
            ViewBag.SortBy = sortBy;
            return View("NewestBookExpanded", newestBookExpanded);
        }

        /// <summary>
        /// Displays an expanded view of the top rated books with options for pagination, search, and sorting.
        /// </summary>
        /// <param name="page">Current page number for pagination. Default is 1.</param>
        /// <param name="pageSize">Number of books per page. Default is 10.</param>
        /// <param name="searchKeyword">Search keyword for filtering books. Default is empty.</param>
        /// <param name="sortBy">Sorting criteria. Default is empty.</param>
        /// <returns>Top Book Expanded view with filtered and sorted book listings.</returns>
        [HttpGet]
        public IActionResult TopBookExpanded(int page = 1, int pageSize = 10, string searchKeyword = "", string sortBy = "")
        {
            var topBookExpanded = _bookService.TopBooksExpanded(page, pageSize, searchKeyword, sortBy);

            return View("TopBookExpanded", topBookExpanded);
        }

        ///<summary>
        /// Displays detailed information of a specific book along with its associated ratings and reviews.
        /// </summary>
        /// <param name="BookId">ID of the book to view.</param>
        /// <returns>View with the book details and its associated ratings and reviews.</returns>
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

        ///<summary>
        /// Displays the view for submitting a rating for a specific book.
        /// </summary>
        /// <param name="BookId">ID of the book to be rated.</param>
        /// <returns>Rate Book view with book records for submitting a rating.</returns>
        [HttpGet]
        public IActionResult RateBook(int BookId)
        {
            var data = _bookService.GetBook(BookId);
            int starRating = _ratingService.GetRatings().Where(x => x.BookId == BookId).Sum(x => x.RateStars);
            ViewData["TotalStar"] = starRating;
            ViewData["Book"] = data;
            return View();
        }

        ///<summary>
        /// Processes the submission of a new rating for a book, validating the email provided.
        /// </summary>
        /// <param name="rating">Rating record submitted by the user.</param>
        /// <returns>Redirects to ViewBookAndReview on success or displays errors if validation fails.</returns>
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

        /// <summary>
        /// Displays a list of books associated with a specific genre, with pagination.
        /// </summary>
        /// <param name="genreName">Name of the genre to display books for.</param>
        /// <param name="page">Current page number for pagination. Default is 1.</param>
        /// <param name="pageSize">Number of books per page. Default is 5.</param>
        /// <returns>Genre List view with books associated with the specified genre, or redirects to Index if the genre is not found.</returns>
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

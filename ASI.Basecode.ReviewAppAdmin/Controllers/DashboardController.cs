using ASI.Basecode.ReviewAppAdmin.Mvc;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
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
        private readonly IRatingService _ratingService;

        public DashboardController (IBookService bookService, IRatingService ratingService, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IConfiguration configuration, IMapper mapper = null)
            : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookService = bookService;
            _ratingService = ratingService;
        }

        /// <summary>
        /// Retrieves and displays lists of newest and top rated books.
        /// </summary>
        /// <returns>Index view with lists of newest and top rated books.</returns>
        [HttpGet]
        public IActionResult Index()
        {
            List<BookViewModel> newest = _bookService.NewestBooks();
            List<BookViewModel> topBooks = _bookService.TopBooks();
            ViewData["TopBooks"] = topBooks;
            return View("Index", newest);
        }

        /// <summary>
        /// Displays an expanded view of the newest books with pagination, search, and sorting capabilities.
        /// </summary>
        /// <param name="page">Page number for pagination. Default is 1.</param>
        /// <param name="pageSize">Number of books per page. Default is 10.</param>
        /// <param name="searchKeyword">Keyword for book search. Default is empty.</param>
        /// <param name="sortBy">Sorting criteria. Default is empty.</param>
        /// <returns>Newest Book Expanded view with search and sorting results.</returns>
        [HttpGet]
        public IActionResult NewestBookExpanded(int page = 1, int pageSize = 10, string searchKeyword = "", string sortBy = "")
        {
            var newestBookExpanded = _bookService.NewestBooksExpanded(page, pageSize, searchKeyword, sortBy);
            ViewBag.SearchKeyword = searchKeyword;
            ViewBag.SortBy = sortBy;
            return View("NewestBookExpanded", newestBookExpanded);
        }

        /// <summary>
        /// Displays an expanded view of the top rated books with pagination, search, and sorting capabilities.
        /// </summary>
        /// <param name="page">Page number for pagination. Default is 1.</param>
        /// <param name="pageSize">Number of books per page. Default is 10.</param>
        /// <param name="searchKeyword">Keyword for book search. Default is empty.</param>
        /// <param name="sortBy">Sorting criteria. Default is empty.</param>
        /// <returns>Top Book Expanded view with search and sorting results.</returns>

        [HttpGet]
        public IActionResult TopBookExpanded(int page = 1, int pageSize = 10, string searchKeyword = "", string sortBy = "")
        {
            var topBookExpanded = _bookService.TopBooksExpanded(page, pageSize, searchKeyword, sortBy);

            return View("TopBookExpanded", topBookExpanded);
        }

        /// <summary>
        /// Displays detailed information about a specific book, including its associated reviews and ratings.
        /// </summary>
        /// <param name="BookId">ID of the book to view.</param>
        /// <returns>View with detailed book information and associated ratings.</returns>
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

        /// <summary>
        /// Displays the view for rating a specific book.
        /// </summary>
        /// <param name="BookId">ID of the book to rate.</param>
        /// <returns>Rate Book view with book details for rating submission.</returns>
        [HttpGet]
        public IActionResult RateBook(int BookId)
        {
            var data = _bookService.GetBook(BookId);
            int starRating = _ratingService.GetRatings().Where(x => x.BookId == BookId).Sum(x => x.RateStars);
            ViewData["TotalStar"] = starRating;
            ViewData["Book"] = data;
            return View();
        }
    }
}

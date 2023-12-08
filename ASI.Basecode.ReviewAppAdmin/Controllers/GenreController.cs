using ASI.Basecode.ReviewAppAdmin.Mvc;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;

namespace ASI.Basecode.ReviewAppAdmin.Controllers
{
    public class GenreController : ControllerBase<GenreController>
    {   
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IConfiguration configuration, IMapper mapper = null)
            : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _genreService = genreService;
        }

        /// <summary>
        /// Retrieves and displays a paginated list of genre records.
        /// </summary>
        /// <param name="page">Page number to display. Default is 1.</param>
        /// <param name="pageSize">Number of records per page. Default is 5.</param>
        /// <returns>View with the list of genres.</returns>
        [HttpGet]
        public IActionResult List(int page = 1, int pageSize = 5)
        {
            var genre = _genreService.PaginatedGenres(page, pageSize);
            return View("List", genre);
        }

        /// <summary>
        /// Displays the view for adding a new genre.
        /// </summary>
        /// <returns>Add Genre view.</returns>
        [HttpGet]
        public IActionResult AddGenre()
        {
            return View();
        }

        /// <summary>
        /// Processes the addition of a new genre record after validating for uniqueness of genre name.
        /// </summary>
        /// <param name="genre">Genre record to add.</param>
        /// <returns>Redirects to the genre list on success, or displays validation errors.</returns>
        [HttpPost]
        public IActionResult AddGenre(GenreViewModel genre)
        {
            if (genre.GenreName == null || genre.GenreName == " ")
            {
                base.ModelState.AddModelError("GenreName", "Genre name is required");
                return View(genre);
            }
            var isExist = _genreService.CheckGenreName(genre.GenreName);
            if(isExist)
            {
                base.ModelState.AddModelError("GenreName", "Genre Name already exists");
                return View(genre);
            }
           
            _genreService.AddGenre(genre, this.UserName);
            TempData["SuccessMessage"] = "Genre successfully added.";
            return RedirectToAction("List");
        }

        /// <summary>
        /// Fetches and displays the details of a genre for editing.
        /// </summary>
        /// <param name="GenreId">ID of the genre to edit.</param>
        /// <returns>Edit Genre view populated with genre's details.</returns>
        [HttpGet]
        public IActionResult EditGenre(int GenreId)
        {
            var data = _genreService.GetGenre(GenreId);
            return View(data);
        }

        /// <summary>
        /// Update genre record to the database
        /// </summary>
        /// <param name="genre"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EditGenre(GenreViewModel genre)
        {
            _genreService.UpdateGenre(genre, this.UserName);
            TempData["SuccessMessage"] = "Genre successfully updated.";
            return RedirectToAction("List");
        }

        /// <summary>
        /// Deletes a genre record from the system.
        /// </summary>
        /// <param name="genre">Genre record to delete.</param>
        /// <returns>Redirects to the genre list after deletion.</returns>
        [HttpPost]
        public IActionResult DeleteGenre(GenreViewModel genre)
        {
            _genreService.DeleteGenre(genre);
            TempData["SuccessMessage"] = "Genre successfully deleted.";
            return RedirectToAction("List");
        }

        /// <summary>
        /// Displays detailed information of a specific genre, including books associated with it.
        /// </summary>
        /// <param name="GenreId">ID of the genre to view.</param>
        /// <param name="page">Page number for pagination. Default is 1.</param>
        /// <param name="pageSize">Number of books per page. Default is 5.</param>
        /// <returns>View with detailed genre information and associated books.</returns>
        [HttpGet]
        public IActionResult ViewGenre(int GenreId, int page = 1, int pageSize = 5)
        {
            var data = _genreService.GetGenre(GenreId);
            var books = _genreService.ViewGenreInBooks(data.GenreName, page, pageSize);
            ViewData["Books"] = books;
            return View("ViewGenre", data);
        }

        /// <summary>
        /// Displays a list of books associated with a specific genre.
        /// </summary>
        /// <param name="genreName">Name of the genre.</param>
        /// <param name="page">Page number for pagination. Default is 1.</param>
        /// <param name="pageSize">Number of books per page. Default is 5.</param>
        /// <returns>Book List view with books of the specified genre.</returns>
        [HttpGet]
        public IActionResult BookList(string genreName, int page = 1, int pageSize = 5)
        {
            var data = _genreService.GetGenreName(genreName);
            if (data != null)
            {
                var books = _genreService.ViewGenreInBooks(data.GenreName, page, pageSize);
                ViewData["Books"] = books;
                return View("BookList", data);
            } else
            {
                return RedirectToAction("List");
            }
        }
    }
}

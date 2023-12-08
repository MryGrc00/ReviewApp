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
        /// List of Genre Records
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult List(int page = 1, int pageSize = 5)
        {
            var genre = _genreService.PaginatedGenres(page, pageSize);
            return View("List", genre);
        }

        /// <summary>
        /// Add Method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddGenre()
        {
            return View();
        }

        /// <summary>
        /// Add genre record to the database
        /// </summary>
        /// <param name="genre"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddGenre(GenreViewModel genre)
        {
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
        /// Edit Method
        /// </summary>
        /// <param name="GenreId"></param>
        /// <returns></returns>
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
        /// Delete genre record to the database
        /// </summary>
        /// <param name="genre"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteGenre(GenreViewModel genre)
        {
            _genreService.DeleteGenre(genre);
            TempData["SuccessMessage"] = "Genre successfully deleted.";
            return RedirectToAction("List");
        }

        /// <summary>
        /// View genre record with books associated
        /// </summary>
        /// <param name="GenreId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ViewGenre(int GenreId, int page = 1, int pageSize = 5)
        {
            var data = _genreService.GetGenre(GenreId);
            var books = _genreService.ViewGenreInBooks(data.GenreName, page, pageSize);
            ViewData["Books"] = books;
            return View("ViewGenre", data);
        }

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

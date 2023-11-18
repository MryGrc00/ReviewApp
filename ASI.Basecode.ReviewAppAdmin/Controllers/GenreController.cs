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
        private readonly IBookService _bookService;

        public GenreController(IGenreService genreService, IBookService bookService, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IConfiguration configuration, IMapper mapper = null)
            : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _genreService = genreService;
            _bookService = bookService;
        }   

        [HttpGet]
        public IActionResult List(int page = 1, int pageSize = 5)
        {
            List<GenreViewModel> data = _genreService.GetGenres();

            int totalCount = data.Count;
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));

            List<GenreViewModel> paginatedGenres = data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalPages"] = totalPages;

            return View("List", paginatedGenres);
        }

        [HttpGet]
        public IActionResult AddGenre()
        {
            return View();
        }

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
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult EditGenre(int GenreId)
        {
            var data = _genreService.GetGenre(GenreId);
            return View(data);
        }

        [HttpPost]
        public IActionResult EditGenre(GenreViewModel genre)
        {
            _genreService.UpdateGenre(genre, this.UserName);
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult DeleteGenre(GenreViewModel genre)
        {
            _genreService.DeleteGenre(genre);
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult ViewGenre(int GenreId, int page = 1, int pageSize = 5)
        {
            var data = _genreService.GetGenre(GenreId);
            List<BookViewModel> BookData = _bookService.GetBooks()
                .Where(x => x.Genre.Split(',').Select(s => s.Trim()).Contains(data.GenreName))
                .ToList();

            int totalCount = BookData.Count;
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            page = Math.Max(1, Math.Min(page, totalPages));

            List<BookViewModel> paginatedBooks = BookData
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["Genre"] = paginatedBooks;

            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalPages"] = totalPages;

            //ViewData["Genre"] = BookData;
            return View(data);
        }
    }
}

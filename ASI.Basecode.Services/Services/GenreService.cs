using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASI.Basecode.Services.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IBookService _bookService;

        public GenreService(IGenreRepository genreRepository, IBookService bookService)
        {
            _genreRepository = genreRepository;
            _bookService = bookService;
        }

        /// <summary>
        /// Fetches all genres, sorted them by the date they were added.
        /// </summary>
        /// <returns>Sorted list of genres.</returns>
        public List<GenreViewModel> GetGenres()
        {
            var data = _genreRepository.GetGenres().Select(x => new GenreViewModel
            {
                GenreId = x.GenreId,
                GenreName = x.GenreName,
                GenreCreatedBy = x.GenreCreatedBy,
                DateAdded = x.DateAdded,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate,
            }).OrderByDescending(x => x.DateAdded).ToList();
            return data;
        }

        /// <summary>
        /// Retrieves genres in a paginated format.
        /// </summary>
        /// <param name="page">Page number to be displayed.</param>
        /// <param name="pageSize">Number of genres per page.</param>
        /// <returns>Paginated genre list.</returns>
        public GenreViewModel PaginatedGenres (int page, int pageSize)
        {
            var data = _genreRepository.GetGenres().Select(x => new GenreViewModel
            {
                GenreId = x.GenreId,
                GenreName = x.GenreName,
                GenreCreatedBy = x.GenreCreatedBy,
                DateAdded = x.DateAdded,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate,
            }).OrderByDescending(x => x.DateAdded).ToList();

            int totalItems = data.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            page = Math.Max(1, Math.Min(page, totalPages));

            List<GenreViewModel> genreOnPage = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new GenreViewModel
            {
                Genres = genreOnPage,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
            };
        }

        /// <summary>
        /// Shows books belonging to a specific genre, paginated.
        /// </summary>
        /// <param name="GenreName">Genre name to filter books.</param>
        /// <param name="page">Page number for display.</param>
        /// <param name="pageSize">Number of books per page.</param>
        /// <returns>Paginated books within the selected genre.</returns>
        public BookViewModel ViewGenreInBooks(string GenreName, int page, int pageSize)
        {
            List<BookViewModel> BookData = _bookService.GetBooks()
                .Where(x => x.Genre.Split(',').Select(s => s.Trim()).Contains(GenreName))
                .ToList();

            int totalCount = BookData.Count;
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            page = Math.Max(1, Math.Min(page, totalPages));

            List<BookViewModel> GenreBooks = BookData
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new BookViewModel
            {
                Books = GenreBooks,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
            };
        }

        /// <summary>
        /// Retrieves records of a genre by its ID.
        /// </summary>
        /// <param name="id">The ID of the genre.</param>
        /// <returns>Records of the specified genre, or null if not found.</returns>
        public GenreViewModel GetGenre(int id)
        {
            var model = _genreRepository.GetGenre(id);
            if (model != null)
            {
                GenreViewModel genre = new()
                {
                    GenreId = model.GenreId,
                    GenreName = model.GenreName,
                    GenreCreatedBy = model.GenreCreatedBy,
                    DateAdded = model.DateAdded,
                    UpdatedBy = model.UpdatedBy,
                    UpdatedDate = model.UpdatedDate,
                };
                return genre;
            }
            return null;
        }

        /// <summary>
        /// Fetches genre record based on its name.
        /// </summary>
        /// <param name="genreName">The name of the genre.</param>
        /// <returns>Genre records if found, otherwise null.</returns>
        public GenreViewModel GetGenreName(string genreName)
        {
            var model = _genreRepository.GetGenreName(genreName);
            if (model != null)
            {
                GenreViewModel genre = new()
                {
                    GenreId = model.GenreId,
                    GenreName = model.GenreName,
                    GenreCreatedBy = model.GenreCreatedBy,
                    DateAdded = model.DateAdded,
                    UpdatedBy = model.UpdatedBy,
                    UpdatedDate = model.UpdatedDate,
                };
                return genre;
            }
            return null;
        }

        /// <summary>
        /// Adds a new genre to the database.
        /// </summary>
        /// <param name="model">The genre record to add.</param>
        /// <param name="name">The name of the person adding the genre.</param>
        public void AddGenre(GenreViewModel model, string name)
        {
            var genre = new Genre();
            genre.GenreName = model.GenreName;
            genre.GenreCreatedBy = name;
            genre.DateAdded = DateTime.Now;
            genre.UpdatedBy = name;
            genre.UpdatedDate = DateTime.Now;
            _genreRepository.AddGenre(genre);
        }

        /// <summary>
        /// Checks if a genre with the specified name already exists.
        /// </summary>
        /// <param name="GenreName">Name of the genre to check.</param>
        /// <returns>True if the genre exists, false otherwise.</returns>
        public bool CheckGenreName(string GenreName)
        {
            var isExist = _genreRepository.GetGenres().Where(x => x.GenreName == GenreName).Any();
            return isExist;
        }

        /// <summary>
        /// Updates an existing genre's records.
        /// </summary>
        /// <param name="model">Updated genre record.</param>
        /// <param name="name">Person making the update.</param>
        public void UpdateGenre(GenreViewModel model, string name)
        {
            Genre genre = _genreRepository.GetGenre(model.GenreId);
            if(genre != null)
            {
                genre.GenreName = model.GenreName;
                genre.UpdatedBy = name;
                genre.UpdatedDate = DateTime.Now;
                _genreRepository.EditGenre(genre);
            }
        }

        /// <summary>
        /// Deletes a genre based on its ID.
        /// </summary>
        /// <param name="model">The genre record to be deleted.</param>
        public void DeleteGenre(GenreViewModel model)
        {
            Genre genre = _genreRepository.GetGenre(model.GenreId);

            if (genre != null)
            {
                _genreRepository.DeleteGenre(genre);
            }
        }
    }
}

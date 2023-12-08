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

        //Validate Genre Name
        public bool CheckGenreName(string GenreName)
        {
            var isExist = _genreRepository.GetGenres().Where(x => x.GenreName == GenreName).Any();
            return isExist;
        }

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

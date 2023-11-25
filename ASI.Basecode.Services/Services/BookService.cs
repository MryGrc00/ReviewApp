using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ASI.Basecode.Services.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IRatingService _ratingService;

    public BookService(IBookRepository bookRepository, IRatingService ratingService)
    {
        _bookRepository = bookRepository;
        _ratingService = ratingService;
    }

    public List<BookViewModel> GetBooks()
    {
        var data = _bookRepository.GetBooks().Select(x => new BookViewModel
        {
            BookId = x.BookId,
            BookImage = x.BookImage,
            Isbn = x.Isbn,
            Title = x.Title,
            Description = x.Description,
            Genre = x.Genre,
            Author = x.Author,
            TotalRating = x.TotalRating,
            CreatedBy = x.CreatedBy,
            DateAdded = x.DateAdded,
            UpdatedBy = x.UpdatedBy,
            UpdatedDate = x.UpdatedDate,
        }).OrderByDescending(x => x.DateAdded).ToList();
        return data;
    }

    public List<BookViewModel> NewestBooksUser()
    {
        DateTime twoWeeksAgo = DateTime.Now.AddDays(-14);
        var data = _bookRepository.GetBooks().Where(x => x.DateAdded >= twoWeeksAgo).Select(x => new BookViewModel
        {
            BookId = x.BookId,
            BookImage = x.BookImage,
            Isbn = x.Isbn,
            Title = x.Title,
            Description = x.Description,
            Genre = x.Genre,
            Author = x.Author,
            TotalRating = x.TotalRating,
            CreatedBy = x.CreatedBy,
            DateAdded = x.DateAdded,
            UpdatedBy = x.UpdatedBy,
            UpdatedDate = x.UpdatedDate,
        }).OrderByDescending(x => x.DateAdded).Take(5).ToList();
        return data;
    }

    public List<BookViewModel> NewestBooks()
    {
        var data = _bookRepository.GetBooks().Select(x => new BookViewModel {
            BookId = x.BookId,
            BookImage = x.BookImage,
            Isbn = x.Isbn,
            Title = x.Title,
            Description = x.Description,
            Genre = x.Genre,
            Author = x.Author,
            TotalRating = x.TotalRating,
            CreatedBy = x.CreatedBy,
            DateAdded = x.DateAdded,
            UpdatedBy = x.UpdatedBy,
            UpdatedDate = x.UpdatedDate,
        }).OrderByDescending(x => x.DateAdded).Take(5).ToList();

        data.ForEach(book => book.SelectedGenres = book.Genre.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());

        return data;
    }

    public BookViewModel ListBooks(int page, int pageSize)
    {
        var data = _bookRepository.GetBooks().Select(x => new BookViewModel
        {
            BookId = x.BookId,
            BookImage = x.BookImage,
            Isbn = x.Isbn,
            Title = x.Title,
            Description = x.Description,
            Genre = x.Genre,
            Author = x.Author,
            TotalRating = x.TotalRating,
            CreatedBy = x.CreatedBy,
            DateAdded = x.DateAdded,
            UpdatedBy = x.UpdatedBy,
            UpdatedDate = x.UpdatedDate,
        }).OrderByDescending(x => x.DateAdded).ToList();

        int totalItems = data.Count;
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        page = Math.Max(1, Math.Min(page, totalPages));

        List<BookViewModel> booksOnPage = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new BookViewModel
        {
            Books = booksOnPage,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
        };
    }

    public BookViewModel NewestBooksExpanded(int page, int pageSize, string searchKeyword, string sortBy)
    {
        var query = _bookRepository.GetBooks();

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(x =>
                x.Title.Contains(searchKeyword) ||
                x.Author.Contains(searchKeyword) ||
                x.Genre.Contains(searchKeyword)
            );
        }

        var data = query.Select(x => new BookViewModel
        {
            BookId = x.BookId,
            BookImage = x.BookImage,
            Isbn = x.Isbn,
            Title = x.Title,
            Description = x.Description,
            Genre = x.Genre,
            Author = x.Author,
            TotalRating = x.TotalRating,
            CreatedBy = x.CreatedBy,
            DateAdded = x.DateAdded,
            UpdatedBy = x.UpdatedBy,
            UpdatedDate = x.UpdatedDate,
        });

        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy.ToLower())
            {
                case "title":
                    data = data.OrderBy(x => x.Title);
                    break;
                case "rating":
                    data = data.OrderByDescending(x => x.TotalRating);
                    break;
                    // Add additional sorting options as needed
            }
        }
        else
        {
            data = data.OrderByDescending(x => x.DateAdded);
        }

        int totalItems = data.Count();
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        page = Math.Max(1, Math.Min(page, totalPages));

        List<BookViewModel> booksOnPage = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new BookViewModel
        {
            Books = booksOnPage,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
        };
    }


    public List<BookViewModel> TopBooks()
    {
        var data = _bookRepository.GetBooks().Select(x => new BookViewModel
        {
            BookId = x.BookId,
            BookImage = x.BookImage,
            Isbn = x.Isbn,
            Title = x.Title,
            Description = x.Description,
            Genre = x.Genre,
            Author = x.Author,
            TotalRating = x.TotalRating,
            CreatedBy = x.CreatedBy,
            DateAdded = x.DateAdded,
            UpdatedBy = x.UpdatedBy,
            UpdatedDate = x.UpdatedDate,
        }).OrderByDescending(x => x.TotalRating).Take(5).ToList();
        return data;
    }

    public BookViewModel TopBooksExpanded(int page, int pageSize)
    {
        var data = _bookRepository.GetBooks().Select(x => new BookViewModel
        {
            BookId = x.BookId,
            BookImage = x.BookImage,
            Isbn = x.Isbn,
            Title = x.Title,
            Description = x.Description,
            Genre = x.Genre,
            Author = x.Author,
            TotalRating = x.TotalRating,
            CreatedBy = x.CreatedBy,
            DateAdded = x.DateAdded,
            UpdatedBy = x.UpdatedBy,
            UpdatedDate = x.UpdatedDate,
        }).OrderByDescending(x => x.TotalRating).ToList();

        int totalItems = data.Count;
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        page = Math.Max(1, Math.Min(page, totalPages));

        List<BookViewModel> booksOnPage = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new BookViewModel
        {
            Books = booksOnPage,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
        };
    }

    public RatingViewModel ViewRatinginBooks (int BookId, int page,  int pageSize)
    {
        var data = _ratingService.GetRatings().Where(x => x.BookId == BookId).ToList();

        int totalItems = data.Count;
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Max(1, Math.Min(page, totalPages));

        List<RatingViewModel> genreOnPage = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new RatingViewModel
        {
            Ratings = genreOnPage,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
        };
    }

    public BookViewModel GetBook(int id)
    {
        var model = _bookRepository.GetBook(id);
        if (model != null)
        {
            var data = _ratingService.GetRatings().Where(x => x.BookId == model.BookId).ToList();
            int totalReview = data.Count();

            BookViewModel book = new()
            {
                BookId = model.BookId,
                BookImage = model.BookImage,
                Isbn = model.Isbn,
                Title = model.Title,
                Description = model.Description,
                Genre = model.Genre,
                Author = model.Author,
                TotalRating = model.TotalRating,
                TotalReview = totalReview,
                CreatedBy = model.CreatedBy,
                DateAdded = model.DateAdded,
                UpdatedBy = model.UpdatedBy,
                UpdatedDate = model.UpdatedDate,
            };
            return book;
        }
        return null;
    }

    public void AddBook(BookViewModel model, string name)
    {
        var url = PathManager.DirectoryPath.BaseUrlHost;
        var book = new Book();
        var bookName = Guid.NewGuid().ToString();
        var sharedImagesPath = PathManager.DirectoryPath.SharedImagesDirectory;
        book.Isbn = model.Isbn;
        book.BookImage = Path.Combine(url, bookName + ".png");
        book.Title = model.Title;
        book.Description = model.Description;
        book.Genre = model.Genre;
        book.Author = model.Author;
        book.TotalRating = 0;
        book.CreatedBy = name;
        book.DateAdded = DateTime.Now;
        book.UpdatedBy = name;
        book.UpdatedDate = DateTime.Now;
        var sharedImageFileName = Path.Combine(sharedImagesPath, bookName) + ".png";
        using(var fileStream = new FileStream(sharedImageFileName, FileMode.Create))
        {
            model.ImageFile.CopyTo(fileStream);
        }
        _bookRepository.AddBook(book);
    }

    //Validate Isbn
    public bool CheckIsbn(string isbn)
    {
        var isExist = _bookRepository.GetBooks().Where(x => x.Isbn == isbn).Any();
        return isExist;
    }

    //Validate Title
    public bool CheckTitle(string title)
    {
        var isExist = _bookRepository.GetBooks().Where(x => x.Title == title).Any();
        return isExist;
    }

    public void UpdateBook(BookViewModel model, string name)
    {
        var url = PathManager.DirectoryPath.BaseUrlHost;
        Book book = _bookRepository.GetBook(model.BookId);
        if(book != null)
        {
            var bookName = Guid.NewGuid().ToString();
            var sharedImagesPath = PathManager.DirectoryPath.SharedImagesDirectory;
            book.Isbn = model.Isbn;
            book.Title = model.Title;
            book.Author = model.Author;
            book.Genre = model.Genre;
            book.Description = model.Description;
            book.UpdatedBy = name;
            book.UpdatedDate = DateTime.Now;
            if (model.ImageFile != null)
            {
                book.BookImage = Path.Combine(url, bookName + ".png");
                var sharedImageFileName = Path.Combine(sharedImagesPath, bookName) + ".png";
                using (var fileStream = new FileStream(sharedImageFileName, FileMode.Create))
                {
                    model.ImageFile.CopyTo(fileStream);

                }
            }
            _bookRepository.EditBook(book);
        }
    }

    public void DeleteBook(BookViewModel model)
    {

        Book book = _bookRepository.GetBook(model.BookId);
        if (book != null)
        {
            _bookRepository.DeleteBook(book);
        }
    }
}

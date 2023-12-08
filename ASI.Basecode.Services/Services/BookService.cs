using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using LinqKit;
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
    /// <summary>
    /// Retrieves all books from the database, along with their average ratings.
    /// </summary>
    /// <returns>A list of books with details and calculated average ratings.</returns>

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

        foreach (var book in data)
        {
            var ratings = _ratingService.GetRatings().Where(r => r.BookId == book.BookId).ToList();
            int totalReview = ratings.Count();

            double averageRating = 0;
            if (totalReview > 0)
            {
                int totalRating = book.TotalRating;
                averageRating = (double)totalRating / totalReview;
                averageRating = Math.Round(averageRating, 1);
            }

            book.AverageRating = averageRating;
        }

        data.ForEach(book => book.SelectedGenres = book.Genre.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());

        return data;
    }
    /// <summary>
    /// Gets the newest books added within the last two weeks, limited to five books.
    /// </summary>
    /// <returns>A list of the five most recently added books.</returns>
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

        foreach (var book in data)
        {
            var ratings = _ratingService.GetRatings().Where(r => r.BookId == book.BookId).ToList();
            int totalReview = ratings.Count();

            double averageRating = 0;
            if (totalReview > 0)
            {
                int totalRating = book.TotalRating;
                averageRating = (double)totalRating / totalReview;
                averageRating = Math.Round(averageRating, 1);
            }

            book.AverageRating = averageRating;
        }

        data.ForEach(book => book.SelectedGenres = book.Genre.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());

        return data;
    }
    /// <summary>
    /// Fetches the five most recently added books from the database.
    /// </summary>
    /// <returns>List of the newest books.</returns>
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

        foreach (var book in data)
        {
            var ratings = _ratingService.GetRatings().Where(r => r.BookId == book.BookId).ToList();
            int totalReview = ratings.Count();

            double averageRating = 0;
            if (totalReview > 0)
            {
                int totalRating = book.TotalRating;
                averageRating = (double)totalRating / totalReview;
                averageRating = Math.Round(averageRating, 1);
            }

            book.AverageRating = averageRating;
        }

        data.ForEach(book => book.SelectedGenres = book.Genre.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());

        return data;
    }
    /// <summary>
    /// Provides a paginated list of books, optimizing data display and navigation.
    /// Calculates average ratings for each book. Allows efficient browsing through large book datasets.
    /// </summary>
    /// <param name="page">The page number to display.</param>
    /// <param name="pageSize">The number of books per page.</param>
    /// <returns>A BookViewModel containing a paginated list of books and their average ratings.</returns>

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

        foreach (var book in booksOnPage)
        {
            var ratings = _ratingService.GetRatings().Where(r => r.BookId == book.BookId).ToList();
            int totalReview = ratings.Count();

            double averageRating = 0;
            if (totalReview > 0)
            {
                int totalRating = book.TotalRating;
                averageRating = (double)totalRating / totalReview;
                averageRating = Math.Round(averageRating, 1);
            }

            book.AverageRating = averageRating;
        }

        data.ForEach(book => book.SelectedGenres = book.Genre.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());

        return new BookViewModel
        {
            Books = booksOnPage,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
        };
    }
    /// <summary>
    /// Retrieves a filtered and sorted list of the newest books based on specified criteria.
    /// Supports searching by title, author, genre, and sorting by different attributes.
    /// </summary>
    /// <param name="page">Page number for pagination.</param>
    /// <param name="pageSize">Number of records per page.</param>
    /// <param name="searchKeyword">Keyword for filtering books.</param>
    /// <param name="sortBy">Sorting criterion for the book list.</param>
    /// <returns>A filtered and sorted list of books.</returns>
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
                case "rating-asc":
                    data = data.OrderBy(x => x.TotalRating);
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

        foreach (var book in booksOnPage)
        {
            var ratings = _ratingService.GetRatings().Where(r => r.BookId == book.BookId).ToList();
            int totalReview = ratings.Count();

            double averageRating = 0;
            if (totalReview > 0)
            {
                int totalRating = book.TotalRating;
                averageRating = (double)totalRating / totalReview;
                averageRating = Math.Round(averageRating, 1);
            }

            book.AverageRating = averageRating;
        }

        data.ForEach(book => book.SelectedGenres = book.Genre.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());

        return new BookViewModel
        {
            Books = booksOnPage,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
        };
    }

    /// <summary>
    /// Retrieves the top five highest-rated books from the database.
    /// </summary>
    /// <returns>List of top five books based on ratings.</returns>
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
        foreach (var book in data)
        {
            var ratings = _ratingService.GetRatings().Where(r => r.BookId == book.BookId).ToList();
            int totalReview = ratings.Count();

            double averageRating = 0;
            if (totalReview > 0)
            {
                int totalRating = book.TotalRating;
                averageRating = (double)totalRating / totalReview;
                averageRating = Math.Round(averageRating, 1);
            }

            book.AverageRating = averageRating;
        }

        data.ForEach(book => book.SelectedGenres = book.Genre.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());

        return data;
    }
    /// <summary>
    /// Provides a paginated and optionally filtered/sorted list of top-rated books.
    /// Facilitates detailed exploration of popular books with customization options.
    /// </summary>
    /// <param name="page">Page number for the list.</param>
    /// <param name="pageSize">Number of books per page.</param>
    /// <param name="searchKeyword">Optional search keyword for filtering.</param>
    /// <param name="sortBy">Optional sorting parameter.</param>
    /// <returns>Paginated list of top books with sorting and filtering options.</returns>
    public BookViewModel TopBooksExpanded(int page, int pageSize, string searchKeyword, string sortBy)
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
                case "rating-asc":
                    data = data.OrderBy(x => x.TotalRating);
                    break;
                    // Add additional sorting options as needed
            }
        }
        else
        {
            data = data.OrderByDescending(x => x.TotalRating);
        }

        int totalItems = data.Count();
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        page = Math.Max(1, Math.Min(page, totalPages));

        List<BookViewModel> booksOnPage = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        foreach (var book in booksOnPage)
        {
            var ratings = _ratingService.GetRatings().Where(r => r.BookId == book.BookId).ToList();
            int totalReview = ratings.Count();

            double averageRating = 0;
            if (totalReview > 0)
            {
                int totalRating = book.TotalRating;
                averageRating = (double)totalRating / totalReview;
                averageRating = Math.Round(averageRating, 1);
            }

            book.AverageRating = averageRating;
        }

        data.ForEach(book => book.SelectedGenres = book.Genre.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());

        return new BookViewModel
        {
            Books = booksOnPage,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
        };
    }
    /// <summary>
    /// Displays ratings for a specific book in a paginated format.
    /// Useful for assessing reader feedback and opinions on a particular book.
    /// </summary>
    /// <param name="BookId">ID of the book to view ratings for.</param>
    /// <param name="page">Page number for the ratings.</param>
    /// <param name="pageSize">Number of ratings per page.</param>
    /// <returns>Paginated view of book ratings.</returns>
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
    /// <summary>
    /// Retrieves detailed record of a specific book by its ID, including its average rating calculated from user reviews.
    /// </summary>
    /// <param name="id">The unique identifier of the book.</param>
    /// <returns>Detailed book record and average rating, or null if the book is not found.</returns>

    public BookViewModel GetBook(int id)
    {
        var model = _bookRepository.GetBook(id);
        if (model != null)
        {
            var data = _ratingService.GetRatings().Where(x => x.BookId == model.BookId).ToList();
            int totalReview = data.Count();

            double averageRating = 0;
            if (totalReview > 0)
            {
                int totalRating = model.TotalRating;
                averageRating = (double)totalRating / totalReview;
                averageRating = Math.Round(averageRating, 1);
            }
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
                AverageRating = averageRating,
                CreatedBy = model.CreatedBy,
                DateAdded = model.DateAdded,
                UpdatedBy = model.UpdatedBy,
                UpdatedDate = model.UpdatedDate,
            };
            return book;
        }
        return null;
    }
    /// <summary>
    /// Adds a new book to the database. Generates a unique name for the book image and stores the book's details.
    /// </summary>
    /// <param name="model">The book record to add.</param>
    /// <param name="name">The name of the person adding the book.</param>

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
        using (var fileStream = new FileStream(sharedImageFileName, FileMode.Create))
        if (model.ImageFile != null)
        {
            model.ImageFile.CopyTo(fileStream);
        }
        _bookRepository.AddBook(book);
    }

    /// <summary>
    /// Checks if the provided ISBN already exists in the database.
    /// </summary>
    /// <param name="isbn">The ISBN to check.</param>
    /// <returns>True if the ISBN exists, false otherwise.</returns>

    public bool CheckIsbn(string isbn)
    {
        var isExist = _bookRepository.GetBooks().Where(x => x.Isbn == isbn).Any();
        return isExist;
    }

    /// <summary>
    /// Verifies if a book with the given title already exists in the database.
    /// </summary>
    /// <param name="title">The title to check.</param>
    /// <returns>True if the title exists, false otherwise.</returns>

    public bool CheckTitle(string title)
    {
        var isExist = _bookRepository.GetBooks().Where(x => x.Title == title).Any();
        return isExist;
    }

    /// <summary>
    /// Verifies if the genre has value.
    /// </summary>
    /// <param name="Genre"></param>
    /// <returns></returns>
    public bool CheckGenre(string Genre)
    {
        if (Genre == null )
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    /// <summary>
    /// Updates the records of an existing book. Replaces the book image if a new one is provided.
    /// </summary>
    /// <param name="model">The updated book record.</param>
    /// <param name="name">The name of the person updating the book.</param>
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
    /// <summary>
    /// Removes a book from the database using its ID.
    /// </summary>
    /// <param name="model">The book record to delete.</param>

    public void DeleteBook(BookViewModel model)
    {

        Book book = _bookRepository.GetBook(model.BookId);
        if (book != null)
        {
            _bookRepository.DeleteBook(book);
        }
    }
}

using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IBookService
    {
        List<BookViewModel> GetBooks();
        List<BookViewModel> NewestBooksUser();
        List<BookViewModel> NewestBooks();
        BookViewModel ListBooks(int page, int pageSize);
        BookViewModel NewestBooksExpanded(int page, int pageSize, string searchKeyword, string sortBy);
        List<BookViewModel> TopBooks();
        BookViewModel TopBooksExpanded(int page, int pageSize, string searchKeyword, string sortBy);
        RatingViewModel ViewRatinginBooks(int BookId, int page, int pageSize);
        BookViewModel GetBook(int id);
        void AddBook(BookViewModel model, string name);
        bool CheckIsbn(String isbn);
        bool CheckTitle(String title);
        bool CheckGenre(string Genre);
        void UpdateBook(BookViewModel model, string name);
        void DeleteBook(BookViewModel model);
    }
}

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
        BookViewModel GetBook(int id);
        void AddBook(BookViewModel model, string name);
        bool CheckIsbn(String isbn);
        bool CheckTitle(String title);
        void UpdateBook(BookViewModel model, string name);
        void DeleteBook(BookViewModel model);
    }
}

using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IBookRepository
    {
        IQueryable<Book> GetBooks();
        Book GetBook(int id);
        void AddBook(Book book);
        void EditBook(Book book);
        void DeleteBook(Book book);
    }
}

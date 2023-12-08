using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IGenreRepository
    {
        IQueryable<Genre> GetGenres();
        Genre GetGenre(int id);
        Genre GetGenreName(string genreName);
        void AddGenre(Genre genre);
        void EditGenre(Genre genre);
        void DeleteGenre(Genre genre);
    }
}

using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class GenreRepository : BaseRepository, IGenreRepository
    {
        public GenreRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IQueryable<Genre> GetGenres()
        {
            return this.GetDbSet<Genre>();
        }

        public Genre GetGenre(int id)
        {
            var genre = this.GetDbSet<Genre>().FirstOrDefault(x => x.GenreId == id);
            return genre;
        }

        public void AddGenre(Genre genre)
        {
            this.GetDbSet<Genre>().Add(genre);
            UnitOfWork.SaveChanges();
        }

        public void EditGenre(Genre genre)
        {
            this.GetDbSet<Genre>().Update(genre);
            UnitOfWork.SaveChanges();
        }

        public void DeleteGenre(Genre genre)
        {
            this.GetDbSet<Genre>().Remove(genre);
            UnitOfWork.SaveChanges();
        }
    }
}

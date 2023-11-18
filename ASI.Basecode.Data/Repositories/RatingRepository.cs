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
    public class RatingRepository : BaseRepository, IRatingRepository
    {
        public RatingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IQueryable<Rating> GetRatings()
        {
            return this.GetDbSet<Rating>();
        }

        public Rating GetRating(int id)
        {
            var rating = this.GetDbSet<Rating>().FirstOrDefault(x => x.BookId == id);
            return rating;
        }

        public void AddRating(Rating rating)
        {
            this.GetDbSet<Rating>().Add(rating);
            UnitOfWork.SaveChanges();
        }

        public void UpdateRating(Rating rating)
        {
            this.GetDbSet<Rating>().Update(rating);
            UnitOfWork.SaveChanges();
        }

        public void DeleteRating(Rating rating)
        {
            this.GetDbSet<Rating>().Remove(rating);
            UnitOfWork.SaveChanges();
        }
    }
}

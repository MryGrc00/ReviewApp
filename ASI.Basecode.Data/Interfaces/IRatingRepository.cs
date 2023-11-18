using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IRatingRepository
    {
        IQueryable<Rating> GetRatings();
        Rating GetRating(int id);
        void AddRating(Rating rating);
        void UpdateRating(Rating rating);
        void DeleteRating(Rating rating);
    }
}

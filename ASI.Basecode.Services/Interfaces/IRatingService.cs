using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IRatingService
    {
        List<RatingViewModel> GetRatings();
        RatingViewModel GetRating(int id);
        void AddRating(RatingViewModel model);
    }
}

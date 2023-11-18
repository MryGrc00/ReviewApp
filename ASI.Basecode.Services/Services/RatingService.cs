using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IBookRepository _bookRepository;

        public RatingService(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public List<RatingViewModel> GetRatings()
        {
            var data = _ratingRepository.GetRatings().Select(x=> new RatingViewModel
            {
                RatingId = x.RatingId,
                BookId = x.BookId,
                Name = x.Name,
                Email = x.Email,
                RateStars = x.RateStars,
                RateComment = x.RateComment,
                DateAdded = x.DateAdded,
            }).OrderByDescending(x => x.DateAdded).ToList();
            return data;
        }

        public RatingViewModel GetRating(int id)
        {
            var model = _ratingRepository.GetRating(id);
            if(model != null)
            {
                RatingViewModel rating = new()
                {
                    RatingId = model.RatingId,
                    BookId = model.BookId,
                    Name = model.Name,
                    Email = model.Email,
                    RateStars = model.RateStars,
                    RateComment = model.RateComment,
                    DateAdded = model.DateAdded,
                };
                return rating;
            }
            return null;
        }

        public void AddRating (RatingViewModel model)
        {
            var rating = new Rating();
            rating.BookId = model.BookId;
            rating.Name = model.Name;
            rating.Email = model.Email;
            rating.RateStars = model.RateStars;
            rating.RateComment = model.RateComment;
            rating.DateAdded = DateTime.Now;
            

            var book = _bookRepository.GetBook(rating.BookId);
            if (book != null)
            {
                book.TotalRating = book.TotalRating + model.RateStars;
                _bookRepository.EditBook(book);
            }
            _ratingRepository.AddRating(rating);
        }
    }
}

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

        public RatingService(IRatingRepository ratingRepository, IBookRepository bookRepository)
        {
            _ratingRepository = ratingRepository;
            _bookRepository = bookRepository;
        }
        /// <summary>
        /// Retrieves a list of all book ratings, including user details and comments, Sorted based on the most recent.
        /// </summary>
        /// <returns>Sorted list of book ratings with user feedback.</returns>
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

        /// <summary>
        /// Retrieves the records of a specific rating, including the user's name, email, and their comments.
        /// </summary>
        /// <param name="id">ID of the rating to retrieve.</param>
        /// <returns>Detailed record about the rating, or null if not found.</returns>
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

        /// <summary>
        /// Adds a new user rating to a book, including comments and star rating, and updates the book's total rating score.
        /// </summary>
        /// <param name="model">Rating record provided by the user.</param>
        public void AddRating (RatingViewModel model)
        {
            var rating = new Rating();
            rating.BookId = model.BookId;
            rating.Name = model.Name;
            rating.Email = model.Email;
            rating.RateStars = model.RateStars;
            rating.RateComment = model.RateComment;
            rating.DateAdded = DateTime.Now;
            

            Book book = _bookRepository.GetBook(rating.BookId);
            if (book != null)
            {
                book.TotalRating = book.TotalRating + model.RateStars;
            }
            _ratingRepository.AddRating(rating);
        }
    }
}

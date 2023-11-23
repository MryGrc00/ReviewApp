using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class RatingViewModel
    {
        public int RatingId { get; set; }
        public int BookId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        public int RateStars { get; set; }

        [Required(ErrorMessage = "Comment is required.")]
        public string RateComment { get; set; }
        public DateTime DateAdded { get; set; }

        public List<RatingViewModel> Ratings { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

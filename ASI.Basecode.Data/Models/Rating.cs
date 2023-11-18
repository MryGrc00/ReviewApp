using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Rating
    {
        public int RatingId { get; set; }
        public int BookId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int RateStars { get; set; }
        public string RateComment { get; set; }
        public DateTime DateAdded { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Genre
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public string GenreCreatedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}

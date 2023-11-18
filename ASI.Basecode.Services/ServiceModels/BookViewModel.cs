using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class BookViewModel
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Isbn is required.")]
        public string Isbn { get; set; }

        public string BookImage { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        public string Author { get; set; }
        public int TotalRating { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public IFormFile ImageFile { get; set; }
        public List<string> SelectedGenres { get; set; }

    }
}

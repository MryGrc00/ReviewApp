using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class GenreViewModel
    {
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Genre Name is required.")]
        public string GenreName { get; set; }
        public string GenreCreatedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}

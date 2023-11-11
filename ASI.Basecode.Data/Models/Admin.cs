using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Admin
    {
        public int AdminId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}

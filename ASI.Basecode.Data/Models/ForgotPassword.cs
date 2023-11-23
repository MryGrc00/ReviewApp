using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class ForgotPassword
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime DateReset { get; set; }
    }
}

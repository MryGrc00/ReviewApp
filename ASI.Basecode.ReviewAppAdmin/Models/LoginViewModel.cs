using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ASI.Basecode.ReviewAppAdmin.Models
{
    /// <summary>
    /// Login View Model
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>ユーザーID</summary>
        [JsonPropertyName("userId")]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        /// <summary>パスワード</summary>
        [JsonPropertyName("password")]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}

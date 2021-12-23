using System.ComponentModel.DataAnnotations;

namespace Backend.Models.ViewModels
{
    public class AuthUser
    {
        [Required(ErrorMessage = "Login must be provided.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Password must be provided")]
        public string Password { get; set; }
    }
}
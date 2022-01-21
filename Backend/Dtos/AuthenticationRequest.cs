using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class AuthenticationRequest
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class User
    {
        public User()
        {
            CreationDate = DateTime.Now;
        }
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "{0} must have between {2} and {1} characters.")]
        [RegularExpression(@"^(?=.{3,20}$)(?![_.-])(?!.*[_.-]{2})[a-zA-Z0-9._-]+(?<![_.-])$",
        ErrorMessage = "{0} must consist of letters, numbers and (-, . _)")]
        public string Login { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} must have between {2} and {1} characters.")]
        [RegularExpression(@"^[\w'\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]]{2,}$",
        ErrorMessage = "{0} entered has not allowed characters.")]
        public string Name { get; set; }
        public string Role { get; set; } = "Customer";

        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$",
        ErrorMessage = "{0} must have at least eight characters, at least one letter, one number and one special character")]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^(([0-9]{3}.[0-9]{3}.[0-9]{3}-[0-9]{2})|([0-9]{11}))$",
        ErrorMessage = "{0} must follow one of the following pattern: 123.456.789-10 or 12345678910")]
        public string Cpf { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CreationDate { get; set; }
        public Address Address { get; set; }
        public int? AddressId { get; set; }
    }
}
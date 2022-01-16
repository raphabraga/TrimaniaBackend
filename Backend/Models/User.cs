using System;
using System.ComponentModel.DataAnnotations;
using Backend.Models.Enums;

namespace Backend.Models
{
    public class User
    {
        public User()
        {
            CreationDate = DateTime.Now;
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Login must be provided.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Login must have between 3 and 20 characters.")]
        [RegularExpression(@"^(?=.{3,20}$)(?![_.-])(?!.*[_.-]{2})[a-zA-Z0-9._-]+(?<![_.-])$",
        ErrorMessage = "Login must consist of letter, numbers and (-, . _)")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Name must be provided.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must have between 3 and 50 charaters.")]
        [RegularExpression(@"^[\w'\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]]{2,}$",
        ErrorMessage = "Name entered has not allowed character.")]
        public string Name { get; set; }
        public string Role { get; set; } = "Customer";

        [Required(ErrorMessage = "Password must be provided.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$",
        ErrorMessage = "Password must have at least eight characters, at least one letter, one number and one special character")]
        public string Password { get; set; }

        [Required(ErrorMessage = "CPF must be provided.")]
        [RegularExpression(@"^(([0-9]{3}.[0-9]{3}.[0-9]{3}-[0-9]{2})|([0-9]{11}))$",
        ErrorMessage = "CPF must follow one of the following pattern: 123.456.789-10 or 12345678910")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "Email must be provided.")]
        [EmailAddress(ErrorMessage = "Valid e-mail must be provided.")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? Birthday { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? CreationDate { get; set; }
        public Address Address { get; set; }
        public int? AddressId { get; set; }
    }
}
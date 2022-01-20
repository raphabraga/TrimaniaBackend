using System;
using System.ComponentModel.DataAnnotations;
using Backend.Models;

namespace Backend.Dtos
{
    public class UpdateUser
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} must have between {2} and {1} characters.")]
        [RegularExpression(@"^[\w'\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]]{2,}$",
        ErrorMessage = "{0} entered has not allowed character.")]
        public string Name { get; set; }
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$",
        ErrorMessage = "{0} must have at least eight characters, at least one letter, one number and one special character")]
        public string Password { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }
        public Address Address { get; set; }

    }
}
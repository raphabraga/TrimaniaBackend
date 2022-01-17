using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models.ViewModels
{
    public class UpdateUser
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must have between 3 and 50 charaters.")]
        [RegularExpression(@"^[\w'\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]]{2,}$",
        ErrorMessage = "Name entered has not allowed character.")]
        public string Name { get; set; }

        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$",
        ErrorMessage = "Password must have at least eight characters, at least one letter, one number and one special character")]
        public string Password { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }
        public Address Address { get; set; }

    }
}
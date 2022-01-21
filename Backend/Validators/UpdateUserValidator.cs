using System;
using Backend.Dtos;
using FluentValidation;

namespace Backend.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserValidator()
        {
            RuleFor(user => user.Name).Matches(@"^[\w'\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]]{2,}$")
            .WithMessage("{PropertyName} entered has not allowed characters.")
            .Length(3, 50);

            RuleFor(user => user.Password).Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$")
            .WithMessage("{PropertyName} must have at least eight characters, at least one letter, one number and one special character");

            RuleFor(user => user.Birthday).Must(date => date != DateTime.MinValue).WithMessage("{PropertyName} is not a valid date");
        }
    }
}
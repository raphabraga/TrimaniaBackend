using System;
using Backend.Dtos;
using FluentValidation;

namespace Backend.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserValidator()
        {
            RuleFor(user => user.Login).NotNull().Matches(@"^(?=.{3,20}$)(?![_.-])(?!.*[_.-]{2})[a-zA-Z0-9._-]+(?<![_.-])$")
            .WithMessage("{PropertyName} must consist of letters, numbers and special characters (_-.)")
            .Length(3, 20);

            RuleFor(user => user.Name).NotNull().Matches(@"^[\w'\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]]{2,}$")
            .WithMessage("{PropertyName} entered has not allowed characters.")
            .Length(3, 50);

            RuleFor(user => user.Password).NotNull().Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$")
            .WithMessage("{PropertyName} must have at least eight characters, at least one letter, one number and one special character");

            RuleFor(user => user.Cpf).NotNull().Matches(@"^(([0-9]{3}.[0-9]{3}.[0-9]{3}-[0-9]{2})|([0-9]{11}))$")
            .WithMessage("{PropertyName} is not a valid Cpf. It must follow one of the following pattern: 123.456.789-10 or 12345678910");

            RuleFor(user => user.Email).NotNull().EmailAddress();

            RuleFor(user => user.Birthday).Must(date => date != DateTime.MinValue).WithMessage("{PropertyName} is not a valid date");
        }
    }
}


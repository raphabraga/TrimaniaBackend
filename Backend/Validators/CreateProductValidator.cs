using Backend.Dtos;
using FluentValidation;

namespace Backend.Validators
{
    public class CreateProductValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductValidator()
        {
            RuleFor(product => product.Name).NotNull().Length(3, 50);

            RuleFor(product => product.Price).NotEmpty().GreaterThanOrEqualTo(0.01m);

            RuleFor(product => product.StockQuantity).NotEmpty().GreaterThanOrEqualTo(1);
        }
    }
}
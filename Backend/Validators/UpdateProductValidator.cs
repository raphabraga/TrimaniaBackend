using Backend.Dtos;
using FluentValidation;

namespace Backend.Validators
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductValidator()
        {
            RuleFor(product => product.Name).Length(3, 50);

            RuleFor(product => product.Price).GreaterThanOrEqualTo(0.01m);

            RuleFor(product => product.StockQuantity).GreaterThanOrEqualTo(1);
        }
    }
}
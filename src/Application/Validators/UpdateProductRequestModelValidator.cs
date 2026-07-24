using CRN.ProductAPI.Application.DTOs.RequestModel;
using FluentValidation;

namespace CRN.ProductAPI.Application.Validators
{
    public class UpdateProductRequestModelValidator : AbstractValidator<UpdateProductRequestModel>
    {
        public UpdateProductRequestModelValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty()
                .WithMessage("Product name is required.")
                .MaximumLength(200)
                .WithMessage("Product name must not exceed 200 characters.");

            RuleFor(x => x.ModifiedBy)
                .NotEmpty()
                .WithMessage("ModifiedBy is required.")
                .MaximumLength(100)
                .WithMessage("ModifiedBy must not exceed 100 characters.");

            RuleFor(x => x.Item)
                .NotEmpty()
                .WithMessage("At least one item is required.");

            RuleForEach(x => x.Item)
                .SetValidator(new AddItemRequestModelValidator());
        }
    }
}

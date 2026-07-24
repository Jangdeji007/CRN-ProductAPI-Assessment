using CRN.ProductAPI.Application.DTOs.RequestModel;
using FluentValidation;

namespace CRN.ProductAPI.Application.Validators
{
    public class AddProductRequestModelValidator : AbstractValidator<AddProductRequestModel>
    {
        public AddProductRequestModelValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty()
                .WithMessage("Product name is required.")
                .MaximumLength(200)
                .WithMessage("Product name must not exceed 200 characters.");

            RuleFor(x => x.CreatedBy)
                .NotEmpty()
                .WithMessage("CreatedBy is required.")
                .MaximumLength(100)
                .WithMessage("CreatedBy must not exceed 100 characters.");

            RuleFor(x => x.Item)
                .NotEmpty()
                .WithMessage("At least one item is required.");

            RuleForEach(x => x.Item)
                .SetValidator(new AddItemRequestModelValidator());
        }
    }
}

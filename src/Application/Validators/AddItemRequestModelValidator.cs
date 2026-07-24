using CRN.ProductAPI.Application.DTOs.RequestModel;
using FluentValidation;

namespace CRN.ProductAPI.Application.Validators
{
    public class AddItemRequestModelValidator : AbstractValidator<AddItemRequestModel>
    {
        public AddItemRequestModelValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Item quantity must be greater than 0.");
        }
    }
}

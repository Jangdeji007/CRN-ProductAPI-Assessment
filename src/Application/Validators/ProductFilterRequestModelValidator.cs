using CRN.ProductAPI.Application.DTOs.RequestModel;
using FluentValidation;

namespace CRN.ProductAPI.Application.Validators
{
    public class ProductFilterRequestModelValidator : AbstractValidator<ProductFilterRequestModel>
    {
        public ProductFilterRequestModelValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("PageNumber must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("PageSize must be between 1 and 100.");

            RuleFor(x => x)
                .Must(x => !x.FromCreatedOn.HasValue
                    || !x.ToCreatedOn.HasValue
                    || x.FromCreatedOn.Value <= x.ToCreatedOn.Value)
                .WithMessage("FromCreatedOn must be less than or equal to ToCreatedOn.");
        }
    }
}

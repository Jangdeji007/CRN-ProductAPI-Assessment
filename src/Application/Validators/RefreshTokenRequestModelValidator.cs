using CRN.ProductAPI.Application.DTOs.RequestModel;
using FluentValidation;

namespace CRN.ProductAPI.Application.Validators
{
    public class RefreshTokenRequestModelValidator : AbstractValidator<RefreshTokenRequestModel>
    {
        public RefreshTokenRequestModelValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}

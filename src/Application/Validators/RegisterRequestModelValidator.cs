using CRN.ProductAPI.Application.DTOs.RequestModel;
using FluentValidation;

namespace CRN.ProductAPI.Application.Validators
{
    public class RegisterRequestModelValidator : AbstractValidator<RegisterRequestModel>
    {
        public RegisterRequestModelValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(256);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .MaximumLength(100);

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(200);
        }
    }
}

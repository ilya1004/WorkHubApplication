using FluentValidation;
using IdentityService.API.Contracts.AuthContracts;

namespace IdentityService.API.Validators.AuthValidators;

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.ResetUrl)
            .NotEmpty().WithMessage("Reset Url is required.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("Invalid url format.");
    }
}
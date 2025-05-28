using FluentValidation;
using IdentityService.API.Contracts.AuthContracts;

namespace IdentityService.API.Validators.AuthValidators;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Email confirmation code is required.");
    }
}
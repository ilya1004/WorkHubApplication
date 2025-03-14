using FluentValidation;
using IdentityService.API.Contracts.UserContracts;

namespace IdentityService.API.Validators.UserValidators;

public class UpdateEmployerProfileRequestValidator : AbstractValidator<UpdateEmployerProfileRequest>
{
    public UpdateEmployerProfileRequestValidator()
    {
        RuleFor(x => x.EmployerProfile.CompanyName)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(200).WithMessage("Company name must not be longer than 200 characters.");

        RuleFor(x => x.EmployerProfile.About)
            .MaximumLength(1000).WithMessage("'About' value must not be longer than 1000 characters.");
    }
}
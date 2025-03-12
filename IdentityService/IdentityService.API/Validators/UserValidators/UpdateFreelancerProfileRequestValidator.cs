using FluentValidation;
using IdentityService.API.Contracts.UserContracts;

namespace IdentityService.API.Validators.UserValidators;

public class UpdateFreelancerProfileRequestValidator : AbstractValidator<UpdateFreelancerProfileRequest>
{
    public UpdateFreelancerProfileRequestValidator()
    {
        RuleFor(x => x.FreelancerProfile.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name must be at most 100 characters long");

        RuleFor(x => x.FreelancerProfile.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name must be at most 100 characters long");

        RuleFor(x => x.FreelancerProfile.About)
            .MaximumLength(1000).WithMessage("'About' value must not be longer than 1000 characters.");
    }
}
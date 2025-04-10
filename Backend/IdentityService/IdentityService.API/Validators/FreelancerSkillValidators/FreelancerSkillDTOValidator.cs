using FluentValidation;
using IdentityService.API.DTOs;

namespace IdentityService.API.Validators.FreelancerSkillValidators;

public class FreelancerSkillDTOValidator : AbstractValidator<FreelancerSkillDataDto>
{
    public FreelancerSkillDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not be longer than 200 characters.");
    }
}
using FluentValidation;
using IdentityService.API.DTOs;

namespace IdentityService.API.Validators.EmployerIndustryValidators;

public class EmployerIndustryDTOValidator : AbstractValidator<EmployerIndustryDto>
{
    public EmployerIndustryDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not be longer than 200 characters.");
    }
}
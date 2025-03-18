using FluentValidation;
using ProjectsService.API.Contracts.FreelancerApplicationContracts;

namespace ProjectsService.API.Validators.FreelancerApplicationValidators;

public class CreateFreelancerApplicationRequestValidator : AbstractValidator<CreateFreelancerApplicationRequest>
{
    public CreateFreelancerApplicationRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotNull().WithMessage("ProjectId is required.");
    }
}
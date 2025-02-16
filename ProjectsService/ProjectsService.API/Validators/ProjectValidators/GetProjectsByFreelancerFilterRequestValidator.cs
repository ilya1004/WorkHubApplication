using FluentValidation;
using ProjectsService.API.Contracts.ProjectContracts;

namespace ProjectsService.API.Validators.ProjectValidators;

public class GetProjectsByFreelancerFilterRequestValidator : AbstractValidator<GetProjectsByFreelancerFilterRequest>
{
    public GetProjectsByFreelancerFilterRequestValidator()
    {
        RuleFor(x => x.FreelancerId)
            .NotEmpty().WithMessage("FreelancerId is required.");
        
        RuleFor(x => x.ProjectStatus)
            .IsInEnum().When(x => x.ProjectStatus.HasValue)
            .WithMessage("ProjectStatus must be a valid Enum value.");

        RuleFor(x => x.PageNo)
            .GreaterThan(0).WithMessage("Page number must be greater than zero.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
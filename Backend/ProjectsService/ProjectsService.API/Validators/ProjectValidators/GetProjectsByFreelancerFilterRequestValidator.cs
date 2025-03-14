using FluentValidation;
using ProjectsService.API.Contracts.ProjectContracts;

namespace ProjectsService.API.Validators.ProjectValidators;

public class GetProjectsByFreelancerFilterRequestValidator : AbstractValidator<GetProjectsByFreelancerFilterRequest>
{
    public GetProjectsByFreelancerFilterRequestValidator()
    {
        RuleFor(x => x.ProjectStatus)
            .IsInEnum().When(x => x.ProjectStatus.HasValue)
            .WithMessage("ProjectStatus must be a valid Enum value.");

        RuleFor(x => x.PageNo)
            .InclusiveBetween(1, 100_000)
            .WithMessage("Page number must be between 1 and 100_000.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("Page size must be between 1 and 1000.");
    }
}
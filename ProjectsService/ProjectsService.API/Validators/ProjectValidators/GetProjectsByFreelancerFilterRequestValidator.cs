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
            .InclusiveBetween(1, 1000)
            .WithMessage("PageNo value must be from 1 to 1000");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("PageSize value must be from 1 to 1000");
    }
}
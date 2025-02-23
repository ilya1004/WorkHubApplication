using FluentValidation;
using ProjectsService.API.Contracts.ProjectContracts;

namespace ProjectsService.API.Validators.ProjectValidators;

public class GetProjectsByEmployerFilterRequestValidator : AbstractValidator<GetProjectsByEmployerFilterRequest>
{
    public GetProjectsByEmployerFilterRequestValidator()
    {
        RuleFor(x => x.UpdatedAtStartDate)
            .LessThan(System.DateTime.UtcNow).WithMessage("UpdatedAtStartDate must be in the past.")
            .When(x => x.UpdatedAtStartDate.HasValue);
        
        RuleFor(x => x.UpdatedAtEndDate)
            .GreaterThan(x => x.UpdatedAtStartDate)
            .WithMessage("UpdatedAtEndDate must be after the UpdatedAtStartDate.")
            .When(x => x.UpdatedAtStartDate.HasValue && x.UpdatedAtEndDate.HasValue);

        RuleFor(x => x.ProjectStatus)
            .IsInEnum().When(x => x.ProjectStatus.HasValue)
            .WithMessage("ProjectStatus must be a valid Enum value.");
        
        RuleFor(x => x.AcceptanceRequestedAndNotConfirmed)
            .Equal(true).WithMessage("AcceptanceRequestedAndNotConfirmed value must be true or null.")
            .When(x => x.AcceptanceRequestedAndNotConfirmed.HasValue);
        
        RuleFor(x => x.PageNo)
            .InclusiveBetween(1, 1000)
            .WithMessage("PageNo value must be from 1 to 1000");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("PageSize value must be from 1 to 1000");
    }
}
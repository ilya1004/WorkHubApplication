using FluentValidation;
using ProjectsService.API.Contracts.ProjectContracts;

namespace ProjectsService.API.Validators.ProjectValidators;

public class GetProjectsByFilterRequestValidator : AbstractValidator<GetProjectsByFilterRequest>
{
    public GetProjectsByFilterRequestValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.BudgetFrom)
            .GreaterThanOrEqualTo(0).WithMessage("BudgetFrom must be greater than or equal to zero.");

        RuleFor(x => x.BudgetTo)
            .GreaterThanOrEqualTo(0).WithMessage("BudgetTo must be greater than or equal to zero.")
            .GreaterThan(x => x.BudgetFrom)
            .When(x => x.BudgetFrom.HasValue && x.BudgetTo.HasValue)
            .WithMessage("BudgetTo must be greater than BudgetFrom.");

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
using FluentValidation;
using ProjectsService.API.Contracts.ProjectContracts;

namespace ProjectsService.API.Validators.ProjectValidators;

public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.Project)
            .NotNull().WithMessage("Project data is required.");

        RuleFor(x => x.Lifecycle)
            .NotNull().WithMessage("Lifecycle data is required.");

        RuleFor(x => x.Project.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Project.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.Project.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than zero.")
            .PrecisionScale(18, 2, true).WithMessage("Budget must have up to 18 digits and 2 decimal places.");

        RuleFor(x => x.Project.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required.")
            .When(x => x.Project.CategoryId.HasValue);

        RuleFor(x => x.Lifecycle.ApplicationsStartDate)
            .NotEmpty().WithMessage("Applications start date is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Applications start date must be in the future.");

        RuleFor(x => x.Lifecycle.ApplicationsDeadline)
            .NotEmpty().WithMessage("Applications deadline is required.")
            .GreaterThan(x => x.Lifecycle.ApplicationsStartDate)
            .WithMessage("Applications deadline must be after the applications start date.");

        RuleFor(x => x.Lifecycle.WorkStartDate)
            .NotEmpty().WithMessage("Work start date is required.")
            .GreaterThan(x => x.Lifecycle.ApplicationsDeadline)
            .WithMessage("Work start date must be after the applications deadline.");

        RuleFor(x => x.Lifecycle.WorkDeadline)
            .NotEmpty().WithMessage("Work deadline is required.")
            .GreaterThan(x => x.Lifecycle.WorkStartDate)
            .WithMessage("Work deadline must be after the work start date.");
    }
}

using FluentValidation;
using ProjectsService.API.Contracts.FreelancerApplicationContracts;

namespace ProjectsService.API.Validators.FreelancerApplicationValidators;

public class GetFreelancerApplicationsByFilterRequestValidator : AbstractValidator<GetMyFreelancerApplicationsByFilterRequest>
{
    public GetFreelancerApplicationsByFilterRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate).When(x => x.EndDate.HasValue)
            .WithMessage("Start date must be before or equal to end date.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate).When(x => x.StartDate.HasValue)
            .WithMessage("End date must be after or equal to start date.");

        RuleFor(x => x.ApplicationStatus)
            .IsInEnum().When(x => x.ApplicationStatus.HasValue)
            .WithMessage("Invalid application status.");

        RuleFor(x => x.PageNo)
            .InclusiveBetween(1, 100_000)
            .WithMessage("Page number must be between 1 and 100_000.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("Page size must be between 1 and 1000.");
    }
}
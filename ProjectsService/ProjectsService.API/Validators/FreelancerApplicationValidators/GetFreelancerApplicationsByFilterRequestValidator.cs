using FluentValidation;
using ProjectsService.API.Contracts.FreelancerApplicationContracts;

namespace ProjectsService.API.Validators.FreelancerApplicationValidators;

public class GetFreelancerApplicationsByFilterRequestValidator : AbstractValidator<GetFreelancerApplicationsByFilterRequest>
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
            .InclusiveBetween(1, 1000)
            .WithMessage("PageNo value must be from 1 to 1000");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("PageSize value must be from 1 to 1000");
    }
}
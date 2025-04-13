using FluentValidation;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetFreelancerMyTransfers;

namespace PaymentsService.Application.Validators.PaymentsValidators;

public class GetFreelancerTransfersQueryValidator : AbstractValidator<GetFreelancerMyTransfersQuery>
{
    public GetFreelancerTransfersQueryValidator()
    {
        RuleFor(x => x.ProjectId)
            .Must(id => id == null || id.Value != Guid.Empty)
            .WithMessage("ProjectId must be a valid GUID or null.");

        RuleFor(x => x.PageNo)
            .InclusiveBetween(1, 100_000)
            .WithMessage("Page number must be between 1 and 100_000.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("Page size must be between 1 and 1000.");
    }
}
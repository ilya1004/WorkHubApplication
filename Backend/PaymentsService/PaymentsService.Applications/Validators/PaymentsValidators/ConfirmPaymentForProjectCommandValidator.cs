using FluentValidation;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.ConfirmPaymentForProject;

namespace PaymentsService.Applications.Validators.PaymentsValidators;

public class ConfirmPaymentForProjectCommandValidator : AbstractValidator<ConfirmPaymentForProjectCommand>
{
    public ConfirmPaymentForProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");
    }
}
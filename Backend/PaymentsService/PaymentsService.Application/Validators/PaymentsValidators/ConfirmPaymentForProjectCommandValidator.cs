using FluentValidation;
using PaymentsService.Application.UseCases.PaymentsUseCases.Commands.ConfirmPaymentForProject;

namespace PaymentsService.Application.Validators.PaymentsValidators;

public class ConfirmPaymentForProjectCommandValidator : AbstractValidator<ConfirmPaymentForProjectCommand>
{
    public ConfirmPaymentForProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");
    }
}
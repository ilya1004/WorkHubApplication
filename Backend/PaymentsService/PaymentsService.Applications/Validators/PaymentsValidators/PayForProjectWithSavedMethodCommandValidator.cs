using FluentValidation;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.PayForProjectWithSavedMethod;

namespace PaymentsService.Applications.Validators.PaymentsValidators;

public class PayForProjectWithSavedMethodCommandValidator : AbstractValidator<PayForProjectWithSavedMethodCommand>
{
    public PayForProjectWithSavedMethodCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");

        RuleFor(x => x.PaymentMethodId)
            .NotEmpty().WithMessage("PaymentMethodId is required")
            .Matches(@"^pm_\w{24}$").WithMessage("Invalid PaymentMethodId format");
    }
}
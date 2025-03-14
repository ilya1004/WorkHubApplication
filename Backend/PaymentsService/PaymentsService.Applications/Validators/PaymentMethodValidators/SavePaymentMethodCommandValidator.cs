using FluentValidation;
using PaymentsService.Applications.UseCases.PaymentMethodUseCases.Commands.SavePaymentMethod;

namespace PaymentsService.Applications.Validators.PaymentMethodValidators;

public class SavePaymentMethodCommandValidator : AbstractValidator<SavePaymentMethodCommand>
{
    public SavePaymentMethodCommandValidator()
    {
        RuleFor(x => x.PaymentMethodId)
            .NotEmpty().WithMessage("PaymentMethodId is required")
            .Matches(@"^pm_\w{24}$").WithMessage("Invalid PaymentMethodId format");
    }
}
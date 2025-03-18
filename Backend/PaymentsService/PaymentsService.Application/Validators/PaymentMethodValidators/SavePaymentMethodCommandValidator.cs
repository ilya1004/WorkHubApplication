using FluentValidation;
using PaymentsService.Application.UseCases.PaymentMethodUseCases.Commands.SavePaymentMethod;

namespace PaymentsService.Application.Validators.PaymentMethodValidators;

public class SavePaymentMethodCommandValidator : AbstractValidator<SavePaymentMethodCommand>
{
    public SavePaymentMethodCommandValidator()
    {
        RuleFor(x => x.PaymentMethodId)
            .NotEmpty().WithMessage("PaymentMethodId is required")
            .Matches(@"^pm_\w{24}$").WithMessage("Invalid PaymentMethodId format");
    }
}
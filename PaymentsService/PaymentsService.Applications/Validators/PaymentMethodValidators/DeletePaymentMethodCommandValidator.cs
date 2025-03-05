using FluentValidation;
using PaymentsService.Applications.UseCases.PaymentMethodUseCases.Commands.DeletePaymentMethod;

namespace PaymentsService.Applications.Validators.PaymentMethodValidators;

public class DeletePaymentMethodCommandValidator : AbstractValidator<DeletePaymentMethodCommand>
{
    public DeletePaymentMethodCommandValidator()
    {
        RuleFor(x => x.PaymentMethodId)
            .NotEmpty().WithMessage("PaymentMethodId is required")
            .Matches(@"^pm_\w{24}$").WithMessage("Invalid PaymentMethodId format");
    }
}
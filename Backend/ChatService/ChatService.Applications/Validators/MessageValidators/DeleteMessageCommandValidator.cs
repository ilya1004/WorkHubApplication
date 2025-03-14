using ChatService.Applications.UseCases.MessageUseCases.DeleteMessage;
using FluentValidation;

namespace ChatService.Applications.Validators.MessageValidators;

public class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
{
    public DeleteMessageCommandValidator()
    {
        RuleFor(x => x.MessageId)
            .NotEmpty().WithMessage("MessageId is required.");
    }
}
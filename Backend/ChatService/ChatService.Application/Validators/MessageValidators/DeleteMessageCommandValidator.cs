using ChatService.Application.UseCases.MessageUseCases.Commands.DeleteMessage;
using FluentValidation;

namespace ChatService.Application.Validators.MessageValidators;

public class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
{
    public DeleteMessageCommandValidator()
    {
        RuleFor(x => x.MessageId)
            .NotEmpty().WithMessage("MessageId is required.");
    }
}
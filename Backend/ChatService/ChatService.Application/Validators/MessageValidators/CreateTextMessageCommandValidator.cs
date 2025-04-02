using ChatService.Application.UseCases.MessageUseCases.Commands.CreateTextMessage;
using FluentValidation;

namespace ChatService.Application.Validators.MessageValidators;

public class CreateTextMessageCommandValidator : AbstractValidator<CreateTextMessageCommand>
{
    public CreateTextMessageCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("ChatId is required.");

        RuleFor(x => x.ReceiverId)
            .NotEmpty().WithMessage("ReceiverId is required.");

        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Text is required.")
            .MaximumLength(10_000).WithMessage("Text must not exceed 10000 characters.");
    }
}
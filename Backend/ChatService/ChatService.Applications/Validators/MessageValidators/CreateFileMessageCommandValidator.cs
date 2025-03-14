using ChatService.Applications.UseCases.MessageUseCases.CreateFileMessage;
using FluentValidation;

namespace ChatService.Applications.Validators.MessageValidators;

public class CreateFileMessageCommandValidator : AbstractValidator<CreateFileMessageCommand>
{
    public CreateFileMessageCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("ChatId is required.");

        RuleFor(x => x.ReceiverId)
            .NotEmpty().WithMessage("ReceiverId is required.");

        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("FileStream is required.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("ContentType is required.");
    }
}
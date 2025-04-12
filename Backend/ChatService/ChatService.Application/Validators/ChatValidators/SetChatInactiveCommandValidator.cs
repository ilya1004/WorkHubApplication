using ChatService.Application.UseCases.ChatUseCases.Commands.SetChatInactive;
using FluentValidation;

namespace ChatService.Application.Validators.ChatValidators;

public class SetChatInactiveCommandValidator : AbstractValidator<SetChatInactiveCommand>
{
    public SetChatInactiveCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ChatId is required.");
    }
}
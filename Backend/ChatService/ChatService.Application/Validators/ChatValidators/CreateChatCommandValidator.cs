using ChatService.Application.UseCases.ChatUseCases.Commands.CreateChat;
using FluentValidation;

namespace ChatService.Application.Validators.ChatValidators;

public class CreateChatCommandValidator : AbstractValidator<CreateChatCommand>
{
    public CreateChatCommandValidator()
    {
        RuleFor(x => x.EmployerId)
            .NotEmpty().WithMessage("EmployerId is required.");

        RuleFor(x => x.FreelancerId)
            .NotEmpty().WithMessage("FreelancerId is required.");

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required.");
    }
}
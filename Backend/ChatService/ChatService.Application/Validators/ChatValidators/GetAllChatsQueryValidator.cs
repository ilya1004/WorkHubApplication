using ChatService.Application.UseCases.ChatUseCases.Queries.GetAllChats;
using FluentValidation;

namespace ChatService.Application.Validators.ChatValidators;

public class GetAllChatsQueryValidator : AbstractValidator<GetAllChatsQuery>
{
    public GetAllChatsQueryValidator()
    {
        RuleFor(x => x.PageNo)
            .InclusiveBetween(1, 100_000)
            .WithMessage("Page number must be between 1 and 100_000.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("Page size must be between 1 and 1000.");
    }
}
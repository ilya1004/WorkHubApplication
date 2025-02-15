using FluentValidation;
using ProjectsService.API.Contracts;

namespace ProjectsService.API.Validators.CategoryValidators;

public class GetPaginatedListRequestValidator : AbstractValidator<GetPaginatedListRequest>
{
    public GetPaginatedListRequestValidator()
    {
        RuleFor(x => x.PageNo)
            .InclusiveBetween(1, 1000)
            .WithMessage("PageNo value must be from 1 to 1000");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("PageSize value must be from 1 to 1000");
    }
}
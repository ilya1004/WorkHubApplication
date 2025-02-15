using FluentValidation;

namespace ProjectsService.API.Validators.CategoryValidators;

public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty")
            .MaximumLength(200).WithMessage("Name cannot be more than 200 characters");
    }
}
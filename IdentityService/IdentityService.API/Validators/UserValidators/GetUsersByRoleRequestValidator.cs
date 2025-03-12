using FluentValidation;
using IdentityService.API.Contracts.UserContracts;
using IdentityService.DAL.Constants;

namespace IdentityService.API.Validators.UserValidators;

public class GetUsersByRoleRequestValidator : AbstractValidator<GetUsersByRoleRequest>
{
    private readonly List<string> _appRoles =
        [AppRoles.AdminRole, AppRoles.EmployerRole, AppRoles.FreelancerRole];

    public GetUsersByRoleRequestValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("UserRole is required.")
            .Must(_appRoles.Contains);

        RuleFor(x => x.PageNo)
            .InclusiveBetween(1, 100_000)
            .WithMessage("Page number must be between 1 and 100_000.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("Page size must be between 1 and 1000.");
    }
}

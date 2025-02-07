using FluentValidation;
using IdentityService.API.Contracts;
using IdentityService.DAL.Constants;

namespace IdentityService.API.Validators;

public class GetUsersByRoleRequestValidator : AbstractValidator<GetUsersByRoleRequest>
{
    private readonly List<string> appRoles = 
        [ AppRoles.AdminRole, AppRoles.EmployerRole, AppRoles.FreelancerRole ];
    public GetUsersByRoleRequestValidator()
    {
        RuleFor(x => x.UserRole)
            .NotEmpty().WithMessage("UserRole is required.")
            .Must(appRoles.Contains);

        RuleFor(x => x.PageNo)
            .InclusiveBetween(1, 999)
            .WithMessage("PageNo value must be from 1 to 999");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize value must be from 1 to 100");
    }
}

using FluentValidation;
using IdentityService.API.Contracts.AuthContracts;

namespace IdentityService.API.Validators.AuthValidators;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token is required.")
            .Must(IsValidJwt).WithMessage("Invalid access token format.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }

    private bool IsValidJwt(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        var parts = token.Split('.');
        return parts.Length == 3;
    }
}
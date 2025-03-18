namespace IdentityService.API.Contracts.AuthContracts;

public sealed record ForgotPasswordRequest(string Email, string ResetUrl);
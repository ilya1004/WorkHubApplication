namespace IdentityService.API.Contracts.AuthContracts;

public sealed record ResetPasswordRequest(string Email, string NewPassword, string Token);
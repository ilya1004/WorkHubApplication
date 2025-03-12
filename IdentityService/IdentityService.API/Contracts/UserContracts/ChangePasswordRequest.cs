namespace IdentityService.API.Contracts.UserContracts;

public sealed record ChangePasswordRequest(string Email, string CurrentPassword, string NewPassword);
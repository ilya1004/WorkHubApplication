namespace IdentityService.API.Contracts;

public sealed record LoginUserRequest(string Email, string Password);

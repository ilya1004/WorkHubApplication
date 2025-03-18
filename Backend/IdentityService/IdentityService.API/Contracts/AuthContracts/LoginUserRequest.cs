namespace IdentityService.API.Contracts.AuthContracts;

public sealed record LoginUserRequest(string Email, string Password);
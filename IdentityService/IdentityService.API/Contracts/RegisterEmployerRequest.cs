namespace IdentityService.API.Contracts;

public sealed record RegisterEmployerRequest(string CompanyName, string Email, string Password);

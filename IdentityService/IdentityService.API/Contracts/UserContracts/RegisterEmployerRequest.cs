namespace IdentityService.API.Contracts.UserContracts;

public sealed record RegisterEmployerRequest(string CompanyName, string Email, string Password);
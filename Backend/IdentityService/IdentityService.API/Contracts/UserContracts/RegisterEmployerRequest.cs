namespace IdentityService.API.Contracts.UserContracts;

public sealed record RegisterEmployerRequest(string UserName, string CompanyName, string Email, string Password);
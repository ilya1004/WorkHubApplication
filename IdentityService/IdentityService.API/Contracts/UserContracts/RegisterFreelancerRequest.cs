namespace IdentityService.API.Contracts.UserContracts;

public sealed record RegisterFreelancerRequest(string UserName, string FirstName, string LastName, string Email, string Password);
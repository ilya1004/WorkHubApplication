namespace IdentityService.API.Contracts;

public sealed record RegisterFreelancerRequest(string UserName, string FirstName, string LastName, string Email, string Password);
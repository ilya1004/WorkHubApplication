namespace IdentityService.API.Contracts;

public record RegisterFreelancerRequest(string FirstName, string LastName, string Email, string Password);
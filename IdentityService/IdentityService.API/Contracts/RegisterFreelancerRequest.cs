namespace IdentityService.API.Contracts;

public record RegisterFreelancerRequest(string UserName, string FirstName, string LastName, string Email, string Password);
namespace IdentityService.API.Constants;

public static class AuthPolicies
{
    public const string AdminPolicy = "AdminPolicy";
    public const string FreelancerPolicy = "FreelancerPolicy";
    public const string EmployerPolicy = "EmployerPolicy";
    public const string FreelancerOrEmployerPolicy = "FreelancerOrEmployerPolicy";
    public const string AdminOrSelfPolicy = "AdminOrSelfPolicy";
}
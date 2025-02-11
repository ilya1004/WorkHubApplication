using Microsoft.AspNetCore.Authorization;

namespace IdentityService.API.AuthorizationPolicies.AdminOrSelfPolicy;

public class AdminOrSelfRequirement : IAuthorizationRequirement { }
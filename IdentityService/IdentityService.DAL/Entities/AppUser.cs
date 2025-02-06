using Microsoft.AspNetCore.Identity;

namespace IdentityService.DAL.Entities;

public class AppUser : IdentityUser<Guid>
{
    public DateTime RegisteredAt { get; set; }
    public string? ImageUrl { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public Guid? FreelancerProfileId { get; set; }
    public FreelancerProfile? FreelancerProfile { get; set; }
    public Guid? EmployerProfileId { get; set; }
    public FreelancerProfile? EmployerProfile { get; set; }
}

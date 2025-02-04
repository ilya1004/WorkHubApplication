using Microsoft.AspNetCore.Identity;

namespace IdentityService.DAL.Entities;

public class User : IdentityUser<Guid>
{
    public DateTime RegisteredAt { get; set; }
    public string ImageUrl { get; set; }
    public Guid FreelancerProfileId { get; set; }
    public FreelancerProfile FreelancerProfile { get; set; }
}

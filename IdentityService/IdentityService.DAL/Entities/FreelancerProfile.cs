using IdentityService.DAL.Primitives;

namespace IdentityService.DAL.Entities;

public class FreelancerProfile : Entity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string About { get; set; }
    public ICollection<FreelancerSkill> Skills { get; set; }
    public string? StripeAccountId { get; set; }
    public Guid UserId { get; set; }
    public AppUser User { get; set; }
}

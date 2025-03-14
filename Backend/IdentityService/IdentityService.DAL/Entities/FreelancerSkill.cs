using IdentityService.DAL.Primitives;

namespace IdentityService.DAL.Entities;

public class FreelancerSkill : Entity
{
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public ICollection<FreelancerProfile> FreelancerProfiles { get; set; }
}
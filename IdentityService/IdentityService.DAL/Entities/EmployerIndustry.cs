using IdentityService.DAL.Primitives;

namespace IdentityService.DAL.Entities;

public class EmployerIndustry : Entity
{
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public Guid EmployerProfileId { get; set; }
}

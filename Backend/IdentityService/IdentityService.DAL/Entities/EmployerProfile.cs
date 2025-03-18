using IdentityService.DAL.Primitives;

namespace IdentityService.DAL.Entities;

public class EmployerProfile : Entity
{
    public string CompanyName { get; set; }
    public string About { get; set; }
    public Guid? IndustryId { get; set; }
    public EmployerIndustry? Industry { get; set; }
    public string? StripeCustomerId { get; set; }
    public Guid UserId { get; set; }
    public AppUser User { get; set; }
}
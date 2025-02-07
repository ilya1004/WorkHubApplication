namespace IdentityService.DAL.Models;

public class EmailVerificationToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Code { get; set; }
    public DateTime CreatedAt { get; set; }
}

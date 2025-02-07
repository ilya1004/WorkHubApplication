using IdentityService.DAL.Models;

namespace IdentityService.DAL.Services.TokenCacheService;

public interface ITokenCacheService
{
    Task SaveTokenAsync(EmailVerificationToken token);
    Task<EmailVerificationToken?> GetTokenAsync(Guid tokenId);
    Task<bool> DeleteTokenAsync(Guid tokenId);
}


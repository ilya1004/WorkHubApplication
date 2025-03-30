using IdentityService.BLL.Abstractions.UserContext;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentUserInfo;

public class GetCurrentUserInfoQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILogger<GetCurrentUserInfoQueryHandler> logger) : IRequestHandler<GetCurrentUserInfoQuery, AppUser>
{
    public async Task<AppUser> Handle(GetCurrentUserInfoQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Getting current user info for user ID: {UserId}", userId);

        var user = await unitOfWork.UsersRepository.GetByIdAsync(
            userId,
            cancellationToken,
            u => u.FreelancerProfile!,
            u => u.EmployerProfile!,
            u => u.FreelancerProfile == null ? null! : u.FreelancerProfile.Skills,
            u => u.EmployerProfile == null ? null! : u.EmployerProfile.Industry!);

        if (user is null)
        {
            logger.LogWarning("User with ID '{UserId}' not found", userId);
            
            throw new NotFoundException($"User with ID '{userId}' not found");
        }

        logger.LogInformation("Successfully retrieved current user info for user ID: {UserId}", userId);
        
        return user;
    }
}
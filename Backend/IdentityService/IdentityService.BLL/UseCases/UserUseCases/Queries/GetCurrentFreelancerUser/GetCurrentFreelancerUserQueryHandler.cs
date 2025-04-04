using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentFreelancerUser;

public class GetCurrentFreelancerUserQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IMapper mapper,
    ILogger<GetCurrentFreelancerUserQueryHandler> logger) : IRequestHandler<GetCurrentFreelancerUserQuery, FreelancerUserDto>
{
    public async Task<FreelancerUserDto> Handle(GetCurrentFreelancerUserQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Getting current user info for user ID: {UserId}", userId);

        var user = await unitOfWork.UsersRepository.GetByIdAsync(
            userId,
            false,
            cancellationToken,
            u => u.FreelancerProfile!,
            u => u.Role,
            u => u.FreelancerProfile == null ? null! : u.FreelancerProfile.Skills);

        if (user is null)
        {
            logger.LogWarning("User with ID '{UserId}' not found", userId);
            
            throw new NotFoundException($"User with ID '{userId}' not found");
        }

        logger.LogInformation("Successfully retrieved current user info for user ID: {UserId}", userId);

        var result = mapper.Map<FreelancerUserDto>(user);
        
        return result;
    }
}
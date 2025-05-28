using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.BLL.DTOs;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentFreelancerUser;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentEmployerUser;

public class GetCurrentEmployerUserQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IMapper mapper,
    ILogger<GetCurrentFreelancerUserQueryHandler> logger) : IRequestHandler<GetCurrentEmployerUserQuery, EmployerUserDto>
{
    public async Task<EmployerUserDto> Handle(GetCurrentEmployerUserQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Getting current user info for user ID: {UserId}", userId);

        var user = await unitOfWork.UsersRepository.GetByIdAsync(
            userId,
            false,
            cancellationToken,
            u => u.EmployerProfile!,
            u => u.Role,
            u => u.EmployerProfile == null ? null! : u.EmployerProfile.Industry!);

        if (user is null)
        {
            logger.LogWarning("User with ID '{UserId}' not found", userId);
            
            throw new NotFoundException($"User with ID '{userId}' not found");
        }

        logger.LogInformation("Successfully retrieved current user info for user ID: {UserId}", userId);

        var result = mapper.Map<EmployerUserDto>(user);
        
        return result;
    }
}
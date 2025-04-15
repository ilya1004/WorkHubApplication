namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;

public class GetUserByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetUserByIdQueryHandler> logger) : IRequestHandler<GetUserByIdQuery, AppUser>
{
    public async Task<AppUser> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting user by ID: {UserId}", request.Id);

        var user = await unitOfWork.UsersRepository.GetByIdAsync(
            request.Id,
            false,
            cancellationToken,
            u => u.FreelancerProfile!,
            u => u.EmployerProfile!,
            u => u.Role,
            u => u.FreelancerProfile == null ? null! : u.FreelancerProfile.Skills,
            u => u.EmployerProfile == null ? null! : u.EmployerProfile.Industry!);

        if (user is null)
        {
            logger.LogWarning("User with ID '{UserId}' not found", request.Id);
            
            throw new NotFoundException($"User with ID '{request.Id}' not found");
        }

        logger.LogInformation("Successfully retrieved user with ID: {UserId}", request.Id);
        
        return user;
    }
}
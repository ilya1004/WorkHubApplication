using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetEmployerUserInfoById;

public class GetEmployerUserInfoByIdQueryHandler(
    ILogger<GetEmployerUserInfoByIdQueryHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<GetEmployerUserInfoByIdQuery, EmployerUserDto>
{
    public async Task<EmployerUserDto> Handle(GetEmployerUserInfoByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting employer info for user ID: {UserId}", request.Id);
        
        var user = await unitOfWork.UsersRepository.GetByIdAsync(
            request.Id,
            false,
            cancellationToken,
            u => u.EmployerProfile!,
            u => u.Role,
            u => u.EmployerProfile == null ? null! : u.EmployerProfile.Industry!);
        
        if (user is null)
        {
            logger.LogWarning("User with ID '{UserId}' not found", request.Id);
                    
            throw new NotFoundException($"User with ID '{request.Id}' not found");
        }
        
        logger.LogInformation("Successfully retrieved employer info for user ID: {UserId}", request.Id);
        
        var result = mapper.Map<EmployerUserDto>(user);
        
        return result;
    }
}
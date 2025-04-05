using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetFreelancerUserInfoById;

public class GetFreelancerUserInfoByIdQueryHandler(
    ILogger<GetFreelancerUserInfoByIdQueryHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<GetFreelancerUserInfoByIdQuery, FreelancerUserDto>
{
    public async Task<FreelancerUserDto> Handle(GetFreelancerUserInfoByIdQuery request, CancellationToken cancellationToken)
    {
         logger.LogInformation("Getting freelancer info for user ID: {UserId}", request.Id);
         
         var user = await unitOfWork.UsersRepository.GetByIdAsync(
             request.Id,
             false,
             cancellationToken,
             u => u.FreelancerProfile!,
             u => u.Role,
             u => u.FreelancerProfile == null ? null! : u.FreelancerProfile.Skills);
        
         if (user is null)
         {
             logger.LogWarning("User with ID '{UserId}' not found", request.Id);
                    
             throw new NotFoundException($"User with ID '{request.Id}' not found");
         }
        
         logger.LogInformation("Successfully retrieved freelancer info for user ID: {UserId}", request.Id);
        
         var result = mapper.Map<FreelancerUserDto>(user);
                
         return result;
    }
}
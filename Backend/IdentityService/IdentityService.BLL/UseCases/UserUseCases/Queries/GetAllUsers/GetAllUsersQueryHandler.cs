using IdentityService.BLL.Models;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllUsersQueryHandler> logger) : IRequestHandler<GetAllUsersQuery, PaginatedResultModel<AppUser>>
{
    public async Task<PaginatedResultModel<AppUser>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all users with pagination - PageNo: {PageNo}, PageSize: {PageSize}", request.PageNo, request.PageSize);

        var offset = (request.PageNo - 1) * request.PageSize;

        var users = await unitOfWork.UsersRepository.PaginatedListAllAsync(
            offset, 
            request.PageSize, 
            cancellationToken,
            u => u.FreelancerProfile!,
            u => u.EmployerProfile!,
            u => u.Role,
            u => u.FreelancerProfile == null ? null! : u.FreelancerProfile.Skills,
            u => u.EmployerProfile == null ? null! : u.EmployerProfile.Industry!);
        
        logger.LogInformation("Retrieved {Count} users from repository", users.Count);

        var usersCount = await unitOfWork.UsersRepository.CountAsync(null, cancellationToken);
        
        logger.LogInformation("Total users count: {TotalCount}", usersCount);

        return new PaginatedResultModel<AppUser>
        {
            Items = users.ToList(),
            TotalCount = usersCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}
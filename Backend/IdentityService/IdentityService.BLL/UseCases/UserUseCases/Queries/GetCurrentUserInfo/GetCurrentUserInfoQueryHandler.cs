using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.DAL.Abstractions.Repositories;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentUserInfo;

public class GetCurrentUserInfoQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<GetCurrentUserInfoQuery, AppUser>
{
    public async Task<AppUser> Handle(GetCurrentUserInfoQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();

        var user = await unitOfWork.UsersRepository.GetByIdAsync(
            userId,
            cancellationToken,
            u => u.FreelancerProfile!,
            u => u.EmployerProfile!,
            u => u.FreelancerProfile == null ? null! : u.FreelancerProfile.Skills,
            u => u.EmployerProfile == null ? null! : u.EmployerProfile.Industry!);

        if (user is null) throw new NotFoundException($"User with ID '{userId}' not found");

        return user;
    }
}
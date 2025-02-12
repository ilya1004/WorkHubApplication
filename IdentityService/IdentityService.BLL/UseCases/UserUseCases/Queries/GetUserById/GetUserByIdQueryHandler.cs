
using IdentityService.DAL.Abstractions.Repositories;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUserByIdQuery, AppUser>
{
    public async Task<AppUser> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UsersRepository.GetByIdAsync(
            request.Id,
            cancellationToken,
            u => u.FreelancerProfile!,
            u => u.EmployerProfile!,
            u => u.FreelancerProfile == null ? null! : u.FreelancerProfile.Skills,
            u => u.EmployerProfile == null ? null! : u.EmployerProfile.Industry!);

        if (user is null)
        {
            throw new NotFoundException($"User with ID '{request.Id}' not found");
        }

        return user;
    }
}

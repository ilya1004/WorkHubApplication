using IdentityService.BLL.Models;
using IdentityService.DAL.Abstractions.Repositories;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllUsersQuery, PaginatedResultModel<AppUser>>
{
    public async Task<PaginatedResultModel<AppUser>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var users = await unitOfWork.UsersRepository.PaginatedListAllAsync(offset, request.PageSize, cancellationToken);

        var usersCount = await unitOfWork.UsersRepository.CountAsync(null, cancellationToken);

        return new PaginatedResultModel<AppUser>
        {
            Items = users.ToList(),
            TotalCount = usersCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}
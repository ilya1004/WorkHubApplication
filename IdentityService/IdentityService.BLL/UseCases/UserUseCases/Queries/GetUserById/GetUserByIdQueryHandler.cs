
namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;

public class GetUserByIdQueryHandler(UserManager<AppUser> userManager) : IRequestHandler<GetUserByIdQuery, AppUser>
{
    public async Task<AppUser> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());

        if (user == null)
        {
            throw new NotFoundException($"User with ID '{request.Id}' not found");
        }

        return user;
    }
}

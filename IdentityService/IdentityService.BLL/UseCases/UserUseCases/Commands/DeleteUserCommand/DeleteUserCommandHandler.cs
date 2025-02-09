namespace IdentityService.BLL.UseCases.UserUseCases.Commands.DeleteUserCommand;

public class DeleteUserCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            throw new NotFoundException($"User with ID '{request.UserId}' not found");
        }

        await userManager.DeleteAsync(user);
    }
}

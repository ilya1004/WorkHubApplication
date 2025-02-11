using IdentityService.BLL.Services.BlobService;
using IdentityService.DAL.Abstractions.Repositories;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.DeleteUserCommand;

public class DeleteUserCommandHandler(
    IUnitOfWork unitOfWork,
    IBlobService blobService) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UsersRepository.GetByIdAsync(
            request.UserId,
            cancellationToken,
            u => u.FreelancerProfile!,
            u => u.EmployerProfile!);

        if (user is null)
        {
            throw new NotFoundException($"User with ID '{request.UserId}' not found");
        }

        if (!string.IsNullOrEmpty(user.ImageUrl))
        {
            await blobService.DeleteAsync(Guid.Parse(user.ImageUrl), cancellationToken);
        }

        await unitOfWork.UsersRepository.DeleteAsync(user, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}

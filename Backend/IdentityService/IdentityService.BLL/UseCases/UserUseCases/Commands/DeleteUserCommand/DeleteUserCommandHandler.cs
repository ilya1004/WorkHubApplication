using IdentityService.BLL.Abstractions.BlobService;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.DeleteUserCommand;

public class DeleteUserCommandHandler(
    IUnitOfWork unitOfWork,
    IBlobService blobService,
    ILogger<DeleteUserCommandHandler> logger) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting user with ID: {UserId}", request.UserId);

        var user = await unitOfWork.UsersRepository.GetByIdAsync(
            request.UserId,
            false,
            cancellationToken,
            u => u.FreelancerProfile!,
            u => u.EmployerProfile!);

        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found", request.UserId);
            
            throw new NotFoundException($"User with ID '{request.UserId}' not found");
        }

        if (!string.IsNullOrEmpty(user.ImageUrl))
        {
            logger.LogInformation("Deleting user image with ID: {ImageId}", user.ImageUrl);
            
            await blobService.DeleteAsync(Guid.Parse(user.ImageUrl), cancellationToken);
        }

        await unitOfWork.UsersRepository.DeleteAsync(user, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        logger.LogInformation("Successfully deleted user with ID: {UserId}", request.UserId);
    }
}
using ProjectsService.Application.Constants;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Domain.Enums;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.DeleteProject;

public class DeleteProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext): IRequestHandler<DeleteProjectCommand>
{
    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(
            request.ProjectId, 
            cancellationToken,
            p => p.Lifecycle);
        
        if (project is null)
        {
            throw new NotFoundException($"Project with ID '{request.ProjectId}' not found");
        }
        
        var isResourceOwned = userContext.GetUserId() == project.EmployerId;
        var isAdmin = userContext.GetUserRole() == AppRoles.AdminRole;

        if (!isResourceOwned && !isAdmin)
        {
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }

        if (project.Lifecycle.Status != ProjectStatus.Cancelled)
        {
            throw new BadRequestException("You need to cancel the project before its removing");
        }
        
        await unitOfWork.ProjectCommandsRepository.DeleteAsync(project, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}
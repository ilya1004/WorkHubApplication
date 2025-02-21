using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateProjectStatus;

public class UpdateProjectStatusCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<UpdateProjectStatusCommand>
{
    public async Task Handle(UpdateProjectStatusCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(
            request.ProjectId, 
            cancellationToken, 
            p => p.Lifecycle);

        if (project is null)
        {
            throw new NotFoundException($"Project with ID '{request.ProjectId}' not found");
        }
        
        var userId = userContext.GetUserId();

        if (project.EmployerId != userId)
        {
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }
        
        project.Lifecycle.Status = request.Status;
        await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}
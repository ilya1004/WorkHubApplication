using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceRequest;

public class UpdateAcceptanceRequestCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<UpdateAcceptanceRequestCommand>
{
    public async Task Handle(UpdateAcceptanceRequestCommand request, CancellationToken cancellationToken)
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
        
        if (project.FreelancerId != userId)
        {
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }
        
        if (project.Lifecycle.Status != ProjectStatus.InProgress && 
            project.Lifecycle.Status != ProjectStatus.Expired)
        {
            throw new BadRequestException("Current project status do not allow you to send acceptance request");
        }
        
        project.Lifecycle.AcceptanceRequested = true;
        
        await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}
namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceStatus;

public class UpdateAcceptanceStatusCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateAcceptanceStatusCommand>
{
    public async Task Handle(UpdateAcceptanceStatusCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(
            request.ProjectId, 
            cancellationToken, 
            p => p.Lifecycle);

        if (project is null)
        {
            throw new NotFoundException($"Project with ID '{request.ProjectId}' not found");
        }

        if (project.EmployerId != request.EmployerId)
        {
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }

        if (request.IsAcceptanceConfirmed)
        {
            project.Lifecycle.AcceptanceConfirmed = true;
            project.Lifecycle.Status = ProjectStatus.Completed;
        }
        else
        {
            project.Lifecycle.AcceptanceRequested = false;
            project.Lifecycle.AcceptanceConfirmed = false;
        }
        
        await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}
namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceRequest;

public class UpdateAcceptanceRequestCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateAcceptanceRequestCommand>
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
        
        if (project.FreelancerId != request.FreelancerId)
        {
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }
        
        project.Lifecycle.AcceptanceRequested = true;
        
        await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}
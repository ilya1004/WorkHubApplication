using ProjectsService.Domain.Enums;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.DeleteProject;

public class DeleteProjectCommandHandler(IUnitOfWork unitOfWork): IRequestHandler<DeleteProjectCommand>
{
    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(
            request.Id, 
            cancellationToken,
            p => p.Lifecycle);
        
        if (project is null)
        {
            throw new NotFoundException($"Project with ID '{request.Id}' not found");
        }

        if (project.Lifecycle.Status != ProjectStatus.Cancelled)
        {
            throw new BadRequestException("You need to cancel the project before deletion");
        }
        
        await unitOfWork.ProjectCommandsRepository.DeleteAsync(project, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}
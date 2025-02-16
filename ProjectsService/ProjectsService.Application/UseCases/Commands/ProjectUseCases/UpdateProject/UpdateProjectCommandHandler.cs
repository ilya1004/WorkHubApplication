using ProjectsService.Domain.Enums;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateProject;

public class UpdateProjectCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateProjectCommand>
{
    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (project is null)
        {
            throw new NotFoundException($"Project with ID '{request.Id}' not found");
        }
        
        var lifecycle = await unitOfWork.LifecycleQueriesRepository.FirstOrDefaultAsync(l => l.ProjectId == project.Id, cancellationToken);
        
        if (lifecycle is null)
        {
            throw new NotFoundException($"Project lifecycle with ProjectId '{project.Id}' not found");
        }

        if (lifecycle.Status != ProjectStatus.Published)
        {
            throw new BadRequestException("You cannot edit this project after the start of accepting applications");
        }

        mapper.Map(project, request.Project);
        mapper.Map(lifecycle, request.Lifecycle);
        
        await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, cancellationToken);
        await unitOfWork.LifecycleCommandsRepository.UpdateAsync(lifecycle, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}
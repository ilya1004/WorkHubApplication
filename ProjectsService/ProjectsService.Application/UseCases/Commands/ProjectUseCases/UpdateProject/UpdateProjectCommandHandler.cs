using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Domain.Enums;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateProject;

public class UpdateProjectCommandHandler(
    IUnitOfWork unitOfWork, 
    IMapper mapper,
    IUserContext userContext) : IRequestHandler<UpdateProjectCommand>
{
    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(request.ProjectId, cancellationToken);

        if (project is null)
        {
            throw new NotFoundException($"Project with ID '{request.ProjectId}' not found");
        }
        
        var userId = userContext.GetUserId();
        
        if (project.EmployerId != userId)
        {
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
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
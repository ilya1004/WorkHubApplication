namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateProject;

public sealed record UpdateProjectCommand(
    Guid EmployerId, 
    Guid ProjectId, 
    UpdateProjectDto Project, 
    LifecycleDto Lifecycle) : IRequest;
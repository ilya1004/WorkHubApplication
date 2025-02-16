namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateProject;

public sealed record UpdateProjectCommand(Guid Id, UpdateProjectDto Project, LifecycleDto Lifecycle) : IRequest;
namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.CreateProject;

public sealed record CreateProjectCommand(ProjectDto Project, LifecycleDto Lifecycle) : IRequest<Guid>;
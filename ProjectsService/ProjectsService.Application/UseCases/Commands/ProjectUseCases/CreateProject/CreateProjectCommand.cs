namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.CreateProject;

public sealed record CreateProjectCommand(Guid EmployerId, ProjectDto Project, LifecycleDto Lifecycle) : IRequest;
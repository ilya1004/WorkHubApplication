namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateProjectStatus;

public sealed record UpdateProjectStatusCommand(Guid EmployerId, Guid ProjectId, ProjectStatus Status) : IRequest;
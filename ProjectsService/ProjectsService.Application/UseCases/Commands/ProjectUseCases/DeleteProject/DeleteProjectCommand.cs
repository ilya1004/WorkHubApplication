namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.DeleteProject;

public sealed record DeleteProjectCommand(Guid EmployerId, Guid ProjectId) : IRequest;
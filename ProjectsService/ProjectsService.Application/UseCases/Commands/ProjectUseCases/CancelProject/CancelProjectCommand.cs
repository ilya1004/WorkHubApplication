namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.CancelProject;

public sealed record CancelProjectCommand(Guid ProjectId) : IRequest;
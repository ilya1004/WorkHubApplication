namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceRequest;

public sealed record UpdateAcceptanceRequestCommand(Guid ProjectId) : IRequest;
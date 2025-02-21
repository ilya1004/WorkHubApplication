namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceStatus;

public sealed record UpdateAcceptanceStatusCommand(Guid ProjectId, bool IsAcceptanceConfirmed) : IRequest;
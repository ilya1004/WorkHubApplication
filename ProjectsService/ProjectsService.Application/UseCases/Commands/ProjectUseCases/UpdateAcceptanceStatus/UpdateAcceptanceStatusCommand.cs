namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceStatus;

public sealed record UpdateAcceptanceStatusCommand(Guid EmployerId, Guid ProjectId, bool IsAcceptanceConfirmed) : IRequest;
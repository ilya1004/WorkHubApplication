namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceRequest;

public sealed record UpdateAcceptanceRequestCommand(Guid FreelancerId, Guid ProjectId) : IRequest;
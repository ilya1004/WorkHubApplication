namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.AcceptFreelancerApplication;

public sealed record AcceptFreelancerApplicationCommand(Guid EmployerId, Guid ProjectId, Guid ApplicationId) : IRequest;
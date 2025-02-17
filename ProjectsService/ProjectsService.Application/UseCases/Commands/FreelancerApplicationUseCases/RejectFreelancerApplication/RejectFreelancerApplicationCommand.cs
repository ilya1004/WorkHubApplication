namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.RejectFreelancerApplication;

public sealed record RejectFreelancerApplicationCommand(Guid EmployerId, Guid ProjectId, Guid ApplicationId) : IRequest;
namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.CreateFreelancerApplication;

public sealed record CreateFreelancerApplicationCommand(Guid FreelancerId, Guid ProjectId) : IRequest;
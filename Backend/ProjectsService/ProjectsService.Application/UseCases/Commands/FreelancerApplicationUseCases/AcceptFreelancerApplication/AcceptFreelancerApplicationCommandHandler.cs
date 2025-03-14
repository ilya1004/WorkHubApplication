using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.AcceptFreelancerApplication;

public class AcceptFreelancerApplicationCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<AcceptFreelancerApplicationCommand>
{
    public async Task Handle(AcceptFreelancerApplicationCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(
            request.ProjectId,
            cancellationToken,
            p => p.Lifecycle);

        if (project is null)
        {
            throw new NotFoundException($"Project with ID '{request.ProjectId}' not found");
        }
        
        var userId = userContext.GetUserId();
        
        if (project.EmployerId != userId)
        {
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }

        if (project.FreelancerId is not null)
        {
            throw new BadRequestException("This project already has freelancer to work on it");
        }

        if (project.Lifecycle.Status != ProjectStatus.AcceptingApplications)
        {
            throw new BadRequestException("You can accept applications to this project only during accepting applications stage");
        }
        
        var freelancerApplication = await unitOfWork.FreelancerApplicationQueriesRepository.GetByIdAsync(
            request.ApplicationId,
            cancellationToken);

        if (freelancerApplication is null)
        {
            throw new NotFoundException($"Freelancer application with ID '{request.ApplicationId}' not found");
        }

        if (freelancerApplication.Status != ApplicationStatus.Pending)
        {
            throw new BadRequestException("Freelancer application status is not pending");
        }

        freelancerApplication.Status = ApplicationStatus.Accepted;

        await unitOfWork.FreelancerApplicationCommandsRepository.UpdateAsync(freelancerApplication, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}
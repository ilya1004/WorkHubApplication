using ProjectsService.Domain.Enums;

namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.RejectFreelancerApplication;

public class RejectFreelancerApplicationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RejectFreelancerApplicationCommand>
{
    public async Task Handle(RejectFreelancerApplicationCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(
            request.ProjectId,
            cancellationToken,
            p => p.Lifecycle);

        if (project is null)
        {
            throw new NotFoundException($"Project with ID '{request.ProjectId}' not found");
        }

        if (project.EmployerId != request.EmployerId)
        {
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }

        if (project.Lifecycle.Status != ProjectStatus.AcceptingApplications)
        {
            throw new BadRequestException("You can reject applications to this project only during accepting applications stage");
        }
        
        var freelancerApplication = await unitOfWork.FreelancerApplicationQueriesRepository.GetByIdAsync(
            request.ApplicationId,
            cancellationToken);

        if (freelancerApplication is null)
        {
            throw new NotFoundException($"Freelancer application with ID '{request.ApplicationId}' not found");
        }
        
        if (freelancerApplication.Status != ApplicationStatus.Accepted)
        {
            throw new BadRequestException("Freelancer application status is not accepted");
        }

        freelancerApplication.Status = ApplicationStatus.Pending;

        await unitOfWork.FreelancerApplicationCommandsRepository.UpdateAsync(freelancerApplication, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}
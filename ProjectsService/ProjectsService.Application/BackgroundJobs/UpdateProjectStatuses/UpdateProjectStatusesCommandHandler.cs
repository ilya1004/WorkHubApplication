using Microsoft.Extensions.Options;
using ProjectsService.Application.Settings;

namespace ProjectsService.Application.BackgroundJobs.UpdateProjectStatuses;

public class UpdateProjectStatusesCommandHandler(
    IUnitOfWork unitOfWork,
    IOptions<ProjectsSettings> options) : IRequestHandler<UpdateProjectStatusesCommand>
{
    public async Task Handle(UpdateProjectStatusesCommand command, CancellationToken cancellationToken)
    {
        var projects = await unitOfWork.ProjectQueriesRepository.ListAsync(
            p => 
                p.Lifecycle.Status != ProjectStatus.Completed && 
                p.Lifecycle.Status != ProjectStatus.Cancelled, 
            cancellationToken, 
            p => p.Lifecycle,
            p => p.FreelancerApplications);

        foreach (var project in projects)
        {
            var lifecycle = project.Lifecycle;
            var previousStatus = lifecycle.Status;
            var now = DateTime.UtcNow;
            
            if (lifecycle.AcceptanceConfirmed)
            {
                lifecycle.Status = ProjectStatus.Completed;
            }
            else if (now > lifecycle.WorkDeadline && 
                lifecycle.Status == ProjectStatus.Expired &&
                lifecycle.UpdatedAt < now.AddDays(options.Value.MaxWorkDeadlineExpirationTimeInDays))
            {
                lifecycle.Status = ProjectStatus.Cancelled;
            }
            else if (now > lifecycle.WorkDeadline && 
                     lifecycle.Status != ProjectStatus.PendingForReview)
            {
                lifecycle.Status = ProjectStatus.Expired;
            }
            else if (now > lifecycle.WorkStartDate.AddDays(options.Value.MaxWorkStartExpirationTimeInDays) &&
                     project.FreelancerId is null)
            {
                lifecycle.Status = ProjectStatus.Cancelled;
            }
            else if (now > lifecycle.WorkStartDate &&
                     IsProjectHasAcceptedFreelancerApplications(project))
            {
                lifecycle.Status = ProjectStatus.InProgress;
                
                UpdateProjectWhenInProgressAsync(project);
            }
            else if (now > lifecycle.ApplicationsDeadline)
            {
                lifecycle.Status = ProjectStatus.WaitingForWorkStart;
            }
            else if (now > lifecycle.ApplicationsStartDate)
            {
                lifecycle.Status = ProjectStatus.AcceptingApplications;
            }
            
            if (previousStatus != lifecycle.Status)
            {
                lifecycle.UpdatedAt = now;
                await unitOfWork.LifecycleCommandsRepository.UpdateAsync(lifecycle, cancellationToken);
            }
            
            if (lifecycle.Status == ProjectStatus.InProgress)
            {
                await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, cancellationToken);
            }
        }

        await unitOfWork.SaveAllAsync(cancellationToken);
    }

    private static bool IsProjectHasAcceptedFreelancerApplications(Project project)
    {
        return project.FreelancerApplications.Any(a => a.Status == ApplicationStatus.Accepted);
    }

    private static void UpdateProjectWhenInProgressAsync(Project project)
    {
        foreach (var application in project.FreelancerApplications)
        {
            if (application.Status != ApplicationStatus.Accepted)
            {
                application.Status = ApplicationStatus.Rejected;
            }
            else
            {
                project.FreelancerId = application.FreelancerId; 
            }
        }
    }
}
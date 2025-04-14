using Microsoft.Extensions.Options;
using ProjectsService.Application.Settings;

namespace ProjectsService.Application.BackgroundJobs.UpdateProjectStatuses;

public class UpdateProjectStatusesCommandHandler(
    IUnitOfWork unitOfWork,
    IOptions<ProjectsSettings> options,
    ILogger<UpdateProjectStatusesCommandHandler> logger) : IRequestHandler<UpdateProjectStatusesCommand>
{
    public async Task Handle(UpdateProjectStatusesCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting update of project statuses");

        var projects = await unitOfWork.ProjectQueriesRepository.ListAsync(
            p => 
                p.Lifecycle.Status != ProjectStatus.Completed && 
                p.Lifecycle.Status != ProjectStatus.Cancelled, 
            cancellationToken, 
            p => p.Lifecycle,
            p => p.FreelancerApplications);

        logger.LogInformation("Found {ProjectCount} projects to process", projects.Count);

        foreach (var project in projects)
        {
            var lifecycle = project.Lifecycle;
            var previousStatus = lifecycle.Status;
            var now = DateTime.UtcNow;
            
            logger.LogInformation("Processing project {ProjectId}, current status: {CurrentStatus}", 
                project.Id, previousStatus);

            if (lifecycle.AcceptanceConfirmed)
            {
                lifecycle.Status = ProjectStatus.Completed;
                
                logger.LogInformation("Project {ProjectId} marked as Completed (AcceptanceConfirmed)", project.Id);
            }
            else if (now > lifecycle.WorkDeadline && 
                lifecycle.Status == ProjectStatus.Expired &&
                lifecycle.UpdatedAt < now.AddDays(options.Value.MaxWorkDeadlineExpirationTimeInDays))
            {
                lifecycle.Status = ProjectStatus.Cancelled;
                
                logger.LogInformation("Project {ProjectId} marked as Cancelled (Expired deadline)", project.Id);
            }
            else if (now > lifecycle.WorkDeadline && 
                     lifecycle.Status != ProjectStatus.PendingForReview)
            {
                lifecycle.Status = ProjectStatus.Expired;
                
                logger.LogInformation("Project {ProjectId} marked as Expired (Passed work deadline)", project.Id);
            }
            else if (now > lifecycle.WorkStartDate && project.FreelancerId is null)
            {
                lifecycle.Status = ProjectStatus.Cancelled;
                
                logger.LogInformation("Project {ProjectId} marked as Cancelled (No freelancer assigned)", project.Id);
            }
            else if (now > lifecycle.WorkStartDate &&
                     IsProjectHasAcceptedFreelancerApplications(project))
            {
                lifecycle.Status = ProjectStatus.InProgress;
                
                logger.LogInformation("Project {ProjectId} marked as InProgress (Work started)", project.Id);
                
                UpdateProjectWhenInProgressAsync(project);
            }
            else if (now > lifecycle.ApplicationsDeadline)
            {
                lifecycle.Status = ProjectStatus.WaitingForWorkStart;
                
                logger.LogInformation("Project {ProjectId} marked as WaitingForWorkStart (Applications deadline passed)", project.Id);
            }
            else if (now > lifecycle.ApplicationsStartDate)
            {
                lifecycle.Status = ProjectStatus.AcceptingApplications;
                
                logger.LogInformation("Project {ProjectId} marked as AcceptingApplications (Applications period started)", project.Id);
            }
            
            if (previousStatus != lifecycle.Status)
            {
                logger.LogInformation("Status changed for project {ProjectId}: {PreviousStatus} -> {NewStatus}", 
                    project.Id, previousStatus, lifecycle.Status);
                
                lifecycle.UpdatedAt = now;
                await unitOfWork.LifecycleCommandsRepository.UpdateAsync(lifecycle, cancellationToken);
            }
            
            if (lifecycle.Status == ProjectStatus.InProgress)
            {
                logger.LogInformation("Updating project {ProjectId} details for InProgress status", project.Id);
                
                await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, cancellationToken);
            }
        }

        logger.LogInformation("Saving all changes for {ProjectCount} projects", projects.Count);
        
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Project statuses update completed");
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
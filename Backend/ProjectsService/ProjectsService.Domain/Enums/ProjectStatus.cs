namespace ProjectsService.Domain.Enums;

public enum ProjectStatus
{
    Published = 0,
    AcceptingApplications = 1,
    WaitingForWorkStart = 2,
    InProgress = 3,
    PendingForReview = 4,
    Completed = 5,
    Expired = 6,
    Cancelled = 7
}
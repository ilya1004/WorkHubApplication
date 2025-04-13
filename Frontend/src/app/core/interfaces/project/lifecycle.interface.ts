export interface Lifecycle {
    id: string;
    createdAt: string;
    updatedAt: string;
    applicationsStartDate: string;
    applicationsDeadline: string;
    workStartDate: string;
    workDeadline: string;
    acceptanceRequested: boolean;
    acceptanceConfirmed: boolean;
    status: ProjectStatus;
}

export enum ProjectStatus {
    Published = 0,
    AcceptingApplications = 1,
    WaitingForWorkStart = 2,
    InProgress = 3,
    PendingForReview = 4,
    Completed = 5,
    Expired = 6,
    Cancelled = 7
}
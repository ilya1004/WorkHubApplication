import {ProjectStatus} from "./project-status.interface";

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
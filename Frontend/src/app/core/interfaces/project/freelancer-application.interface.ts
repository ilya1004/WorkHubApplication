import {Project} from './project.interface';

export interface FreelancerApplication {
  id: string;
  createdAt: string;
  status: ApplicationStatus;
  projectId: string;
  project: Project | null;
  freelancerId: string;
}

export enum ApplicationStatus {
  Pending = 0,
  Accepted = 1,
  Rejected = 2,
}

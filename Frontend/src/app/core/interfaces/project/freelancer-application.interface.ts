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
  pending = 0,
  accepted = 1,
  rejected = 2,
}

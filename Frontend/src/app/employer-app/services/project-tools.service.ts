import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {PaginatedResult} from "../../core/interfaces/common/paginated-result.interface";
import {Project} from "../../core/interfaces/project/project.interface";
import {FreelancerApplication} from "../../core/interfaces/project/freelancer-application.interface";
import {FreelancerUser} from "../../core/interfaces/freelancer/freelancer-user.interface";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class ProjectToolsService {

  constructor(
    private httpClient: HttpClient
  ) { }
  
  getEmployerProjects(pageNo: number, pageSize: number): Observable<PaginatedResult<Project>> {
    const params = new HttpParams()
      .set('PageNo', pageNo.toString())
      .set('PageSize', pageSize.toString());
    return this.httpClient.get<PaginatedResult<Project>>(
      `${environment.PROJECTS_SERVICE_API_URL}projects/my-employer-projects-filter`,
      { params }
    );
  }
  
  createProject(data: {
    project: {
      title: string;
      description: string;
      budget: number;
      categoryId: string | null;
    };
    lifecycle: {
      applicationsStartDate: string;
      applicationsDeadline: string;
      workStartDate: string;
      workDeadline: string;
    };
  }): Observable<{ projectId: string }> {
    return this.httpClient.post<{ projectId: string }>(
      `${environment.PROJECTS_SERVICE_API_URL}projects`,
      data
    );
  }
  
  updateProject(projectId: string, data: {
    project: {
      title: string,
      description: string,
      budget: number,
      categoryId: string | null,
    },
    lifecycle: {
      applicationsStartDate: string;
      applicationsDeadline: string;
      workStartDate: string;
      workDeadline: string;
    }
  }): Observable<void> {
    return this.httpClient.put<void>(
      `${environment.PROJECTS_SERVICE_API_URL}projects/${projectId}`,
      data
    );
  }
  
  cancelProject(projectId: string): Observable<void> {
    return this.httpClient.patch<void>(
      `${environment.PROJECTS_SERVICE_API_URL}projects/${projectId}/cancel-project`, {}
    );
  }
  
  getApplicationsByProject(projectId: string, pageNo: number, pageSize: number): Observable<PaginatedResult<FreelancerApplication>> {
    const params = new HttpParams()
      .set('PageNo', pageNo.toString())
      .set('PageSize', pageSize.toString());
    return this.httpClient.get<PaginatedResult<FreelancerApplication>>(
      `${environment.PROJECTS_SERVICE_API_URL}freelancer-applications/by-project/${projectId}`,
      { params }
    );
  }
  
  getFreelancerInfo(userId: string): Observable<FreelancerUser> {
    return this.httpClient.get<FreelancerUser>(
      `${environment.IDENTITY_SERVICE_API_URL}users/freelancer-info/${userId}`
    );
  }
  
  acceptApplication(projectId: string, applicationId: string): Observable<void> {
    return this.httpClient.patch<void>(
      `${environment.PROJECTS_SERVICE_API_URL}freelancer-applications/${applicationId}/accept-application/${projectId}`, {}
    );
  }
  
  rejectApplication(projectId: string, applicationId: string): Observable<void> {
    return this.httpClient.patch<void>(
      `${environment.PROJECTS_SERVICE_API_URL}freelancer-applications/${applicationId}/reject-application/${projectId}`, {}
    );
  }
  
  deleteProject(projectId: string): Observable<void> {
    return this.httpClient.delete<void>(
      `${environment.PROJECTS_SERVICE_API_URL}projects/${projectId}`
    );
  }
}
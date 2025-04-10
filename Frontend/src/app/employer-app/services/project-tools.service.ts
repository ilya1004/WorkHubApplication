import { Injectable } from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {PaginatedResult} from "../../core/interfaces/common/paginated-result.interface";
import {Project} from "../../core/interfaces/project/project.interface";
import {IDENTITY_SERVICE_API_URL, PROJECTS_SERVICE_API_URL} from "../../core/data/constants";
import {FreelancerApplication} from "../../core/interfaces/project/freelancer-application.interface";
import {FreelancerUser} from "../../core/interfaces/freelancer/freelancer-user.interface";

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
      `${PROJECTS_SERVICE_API_URL}projects/my-employer-projects-filter`,
      { params }
    );
  }
  
  createProject(
    request: {
      project: {
        title: string,
        description: string | null,
        budget: number,
        categoryId: string | null
      },
      lifecycle: {
        applicationsStartDate: Date,
        applicationsDeadline: Date,
        workStartDate: Date,
        workDeadline: Date
      }
    }): Observable<void> {
    return this.httpClient.post<void>(
      `${PROJECTS_SERVICE_API_URL}`,
      request
    );
  }
  
  updateProject(
    projectId: string,
    request: {
      project: {
        title: string,
        description: string | null,
        budget: number,
        categoryId: string | null
      },
      lifecycle: {
        applicationsStartDate: Date,
        applicationsDeadline: Date,
        workStartDate: Date,
        workDeadline: Date
      }
    }): Observable<void> {
    return this.httpClient.put<void>(
      `${PROJECTS_SERVICE_API_URL}projects/${projectId}`,
      request
    );
  }
  
  cancelProject(projectId: string): Observable<void> {
    return this.httpClient.patch<void>(
      `${PROJECTS_SERVICE_API_URL}projects/${projectId}/cancel-project`, {}
    );
  }
  
  getApplicationsByProject(projectId: string, pageNo: number, pageSize: number): Observable<PaginatedResult<FreelancerApplication>> {
    const params = new HttpParams()
      .set('PageNo', pageNo.toString())
      .set('PageSize', pageSize.toString());
    return this.httpClient.get<PaginatedResult<FreelancerApplication>>(
      `${PROJECTS_SERVICE_API_URL}freelancer-applications/by-project/${projectId}`,
      { params }
    );
  }
  
  getFreelancerInfo(userId: string): Observable<FreelancerUser> {
    return this.httpClient.get<FreelancerUser>(
      `${IDENTITY_SERVICE_API_URL}users/freelancer-info/${userId}`
    );
  }
  
  acceptApplication(projectId: string, applicationId: string): Observable<void> {
    return this.httpClient.patch<void>(
      `${PROJECTS_SERVICE_API_URL}freelancer-applications/${applicationId}/accept-application/${projectId}`, {}
    );
  }
  
  rejectApplication(projectId: string, applicationId: string): Observable<void> {
    return this.httpClient.patch<void>(
      `${PROJECTS_SERVICE_API_URL}freelancer-applications/${applicationId}/reject-application/${projectId}`, {}
    );
  }
}
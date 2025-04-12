import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {HttpClient, HttpParams} from "@angular/common/http";
import {environment} from "../../../../environments/environment";
import {ApplicationStatus, FreelancerApplication} from "../../interfaces/project/freelancer-application.interface";
import {PaginatedResult} from "../../interfaces/common/paginated-result.interface";

@Injectable({
  providedIn: 'root'
})
export class FreelancerApplicationsService {

  constructor(
    private httpClient: HttpClient,
  ) { }
  
  createFreelancerApplication(projectId: string): Observable<void> {
    const payload = { projectId };
    return this.httpClient.post<void>(
      `${environment.PROJECTS_SERVICE_API_URL}freelancer-applications`,
      payload
    );
  }
  
  cancelFreelancerApplication(applicationId: string): Observable<void> {
    return this.httpClient.delete<void>(
      `${environment.PROJECTS_SERVICE_API_URL}freelancer-applications/${applicationId}`
    );
  }
  
  getFreelancerApplications(params: {
    startDate?: string | null;
    endDate?: string | null;
    status?: ApplicationStatus | null;
    pageNo: number;
    pageSize: number;
  }): Observable<PaginatedResult<FreelancerApplication>> {
    let httpParams = new HttpParams()
      .set('PageNo', params.pageNo.toString())
      .set('PageSize', params.pageSize.toString());
    
    if (params.startDate) {
      httpParams = httpParams.set('StartDate', params.startDate);
    }
    if (params.endDate) {
      httpParams = httpParams.set('EndDate', params.endDate);
    }
    if (params.status !== null && params.status !== undefined) {
      httpParams = httpParams.set('ApplicationStatus', params.status.toString());
    }
    
    return this.httpClient.get<PaginatedResult<FreelancerApplication>>(
      `${environment.PROJECTS_SERVICE_API_URL}freelancer-applications/my-applications-filter`,
      { params: httpParams }
    );
  }
}

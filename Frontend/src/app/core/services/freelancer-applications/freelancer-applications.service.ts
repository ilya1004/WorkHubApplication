import {Injectable} from '@angular/core';
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
  
  getFreelancerApplicationsByFilter(filter: {
    startDate?: string | null;
    endDate?: string | null;
    applicationStatus?: number | null;
    pageNo: number;
    pageSize: number;
  }): Observable<PaginatedResult<FreelancerApplication>> {
    let params = new HttpParams()
      .set('PageNo', filter.pageNo.toString())
      .set('PageSize', filter.pageSize.toString());
    
    if (filter.startDate) {
      params = params.set('StartDate', filter.startDate);
    }
    if (filter.endDate) {
      params = params.set('EndDate', filter.endDate);
    }
    if (filter.applicationStatus !== null && filter.applicationStatus !== undefined) {
      params = params.set('ApplicationStatus', filter.applicationStatus.toString());
    }
    
    return this.httpClient.get<PaginatedResult<FreelancerApplication>>(
      `${environment.PROJECTS_SERVICE_API_URL}freelancer-applications/by-filter`,
      { params }
    );
  }
}

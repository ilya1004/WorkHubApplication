import {Injectable} from '@angular/core';
import {Observable} from "rxjs";
import {PaginatedResult} from "../../interfaces/common/paginated-result.interface";
import {Project} from "../../interfaces/project/project.interface";
import {HttpClient, HttpParams} from "@angular/common/http";
import {environment} from "../../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class ProjectsService {

  constructor(
    private httpClient: HttpClient,
  ) { }

  getProjectsByFilter(filter: {
    title: string | null,
    budgetFrom: number | null,
    budgetTo: number | null,
    categoryId: string | null,
    employerId: string | null,
    projectStatus: string | null,
    pageNo: number,
    pageSize: number
  }): Observable<PaginatedResult<Project>> {

    let params = new HttpParams()
      .set('pageNo', filter.pageNo.toString())
      .set('pageSize', filter.pageSize.toString());

    if (filter.title !== null) {
      params = params.set('title', filter.title);
    }

    if (filter.budgetFrom !== null) {
      params = params.set('budgetFrom', filter.budgetFrom.toString());
    }

    if (filter.budgetTo !== null) {
      params = params.set('budgetTo', filter.budgetTo.toString());
    }

    if (filter.categoryId !== null) {
      params = params.set('categoryId', filter.categoryId);
    }

    if (filter.employerId !== null) {
      params = params.set('employerId', filter.employerId);
    }

    if (filter.projectStatus !== null) {
      params = params.set('projectStatus', filter.projectStatus);
    }

    return this.httpClient.get<PaginatedResult<Project>>(
      `${environment.PROJECTS_SERVICE_API_URL}projects/by-filter`,
      { params }
    );
  }
  
  getProjectById(projectId: string): Observable<Project> {
    return this.httpClient.get<Project>(
      `${environment.PROJECTS_SERVICE_API_URL}projects/${projectId}`
    );
  }
  
  requestProjectAcceptance(projectId: string): Observable<void> {
    return this.httpClient.patch<void>(
      `${environment.PROJECTS_SERVICE_API_URL}projects/${projectId}/request-acceptance`, {}
    );
  }
  
  setProjectAcceptanceStatus(projectId: string, status: boolean): Observable<void> {
    return this.httpClient.patch<void>(
      `${environment.PROJECTS_SERVICE_API_URL}projects/${projectId}/set-acceptance-status/${status}`, {}
    );
  }
}

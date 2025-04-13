import {Injectable} from '@angular/core';
import {Observable} from "rxjs";
import {PaginatedResult} from "../../core/interfaces/common/paginated-result.interface";
import {Project} from "../../core/interfaces/project/project.interface";
import {HttpClient, HttpParams} from "@angular/common/http";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class EmployerProjectsService {

  constructor(
    private httpClient: HttpClient,
  ) { }

  getMyEmployerProjects(filter: {
    updatedAtStartDate: Date | null,
    updatedAtEndDate: Date | null,
    projectStatus: string | null,
    acceptanceRequestedAndNotConfirmed: boolean | null,
    pageNo: number,
    pageSize: number
  }): Observable<PaginatedResult<Project>> {

    let params = new HttpParams()
      .set('pageNo', filter.pageNo.toString())
      .set('pageSize', filter.pageSize.toString());

    if (filter.updatedAtStartDate !== null) {
      params = params.set('updatedAtStartDate', filter.updatedAtStartDate.toISOString());
    }

    if (filter.updatedAtEndDate !== null) {
      params = params.set('updatedAtEndDate', filter.updatedAtEndDate.toISOString());
    }

    if (filter.projectStatus !== null) {
      params = params.set('projectStatus', filter.projectStatus);
    }
    
    if (filter.acceptanceRequestedAndNotConfirmed) {
      params = params.set('AcceptanceRequestedAndNotConfirmed', 'true');
    }

    return this.httpClient.get<PaginatedResult<Project>>(
      `${environment.PROJECTS_SERVICE_API_URL}projects/my-employer-projects-filter`,
      { params }
    );
  }
}

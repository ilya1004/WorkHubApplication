import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {PaginatedResult} from "../../core/interfaces/paginated-result.interface";
import {Project} from "../../freelancer-app/interfaces/my-projects/project.interface";
import {HttpClient, HttpParams} from "@angular/common/http";
import {PROJECTS_SERVICE_API_URL} from "../../core/constants";

@Injectable({
  providedIn: 'root'
})
export class MyProjectsService {

  constructor(
    private httpClient: HttpClient,
  ) { }

  getMyEmployerProjects(filter: {
    updatedAtStartDate: string | null,
    updatedAtEndDate: string | null,
    projectStatus: number | null,
    acceptanceRequestedAndNotConfirmed: boolean | null,
    pageNo: number,
    pageSize: number
  }): Observable<PaginatedResult<Project>> {

    let params = new HttpParams()
      .set('pageNo', filter.pageNo.toString())
      .set('pageSize', filter.pageSize.toString());

    if (filter.updatedAtStartDate !== null) {
      params = params.set('updatedAtStartDate', filter.updatedAtStartDate);
    }

    if (filter.updatedAtEndDate !== null) {
      params = params.set('updatedAtEndDate', filter.updatedAtEndDate);
    }

    if (filter.projectStatus !== null) {
      params = params.set('projectStatus', filter.projectStatus.toString());
    }

    if (filter.acceptanceRequestedAndNotConfirmed !== null) {
      params = params.set('acceptanceRequestedAndNotConfirmed', filter.acceptanceRequestedAndNotConfirmed.toString());
    }

    return this.httpClient.get<PaginatedResult<Project>>(
      `${PROJECTS_SERVICE_API_URL}projects/my-employer-projects-filter`,
      { params }
    );
  }
}

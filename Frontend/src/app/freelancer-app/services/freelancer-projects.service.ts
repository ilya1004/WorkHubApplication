import {Injectable} from '@angular/core';
import {PaginatedResult} from "../../core/interfaces/common/paginated-result.interface";
import {Project} from "../../core/interfaces/project/project.interface";
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class FreelancerProjectsService {

  constructor(
      private httpClient: HttpClient
  ) { }

  getMyFreelancerProjects(filter: {
    projectStatus: string | null,
    employerId: string | null,
    pageNo: number,
    pageSize: number
  } ): Observable<PaginatedResult<Project>> {
    let params = new HttpParams()
        .set('pageNo', filter.pageNo.toString())
        .set('pageSize', filter.pageSize.toString());

    if (filter.projectStatus !== null) {
      params = params.set('projectStatus', filter.projectStatus.toString());
    }

    if (filter.employerId !== null) {
      params = params.set('employerId', filter.employerId);
    }

    return this.httpClient.get<PaginatedResult<Project>>(
        `${environment.PROJECTS_SERVICE_API_URL}projects/my-freelancer-projects-filter`,
        { params },
    );
  }
}

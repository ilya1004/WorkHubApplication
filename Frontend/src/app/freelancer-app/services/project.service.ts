import { Injectable } from '@angular/core';
import {PaginatedResult} from "../../core/interfaces/paginated-result.interface";
import {Project} from "../interfaces/my-projects/project.interface";
import {PROJECTS_SERVICE_API_URL} from "../../core/constants";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class ProjectService {

  constructor(
      private httpClient: HttpClient
  ) { }

  getProjectById(projectId: string): Observable<Project> {
    return this.httpClient.get<Project>(
        `${PROJECTS_SERVICE_API_URL}projects/${projectId}`,
    );
  }
}

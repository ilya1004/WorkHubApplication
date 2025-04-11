import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {PROJECTS_SERVICE_API_URL} from "../../data/constants";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class FreelancerApplicationsService {

  constructor(
    private httpClient: HttpClient,
  ) { }
  
  createFreelancerApplication(projectId: string): Observable<void> {
    const payload = { projectId };
    return this.httpClient.post<void>(`${PROJECTS_SERVICE_API_URL}freelancer-applications`, payload);
  }
  
  cancelFreelancerApplication(applicationId: string): Observable<void> {
    return this.httpClient.delete<void>(`${PROJECTS_SERVICE_API_URL}freelancer-applications/${applicationId}`);
  }
}

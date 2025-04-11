import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class FreelancerApplicationsService {

  constructor(
    private httpClient: HttpClient,
  ) { }
  
  createFreelancerApplication(projectId: string): Observable<void> {
    const payload = { projectId };
    return this.httpClient.post<void>(`${environment.PROJECTS_SERVICE_API_URL}freelancer-applications`, payload);
  }
  
  cancelFreelancerApplication(applicationId: string): Observable<void> {
    return this.httpClient.delete<void>(`${environment.PROJECTS_SERVICE_API_URL}freelancer-applications/${applicationId}`);
  }
}

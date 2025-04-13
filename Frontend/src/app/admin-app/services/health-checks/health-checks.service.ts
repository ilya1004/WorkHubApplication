import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {HealthCheckResponse} from "../../interfaces/health-checks/health-checks.interface";
import {environment} from "../../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class HealthChecksService {
  private readonly endpoints = [
    environment.PROJECTS_SERVICE_HEALTH_URL,
    environment.IDENTITY_SERVICE_HEALTH_URL,
    environment.PAYMENTS_SERVICE_HEALTH_URL,
    environment.CHAT_SERVICE_HEALTH_URL
  ];
  
  constructor(
    private http: HttpClient
  ) {}
  
  getHealthChecks(): Observable<HealthCheckResponse>[] {
    return this.endpoints.map(endpoint =>
      this.http.get<HealthCheckResponse>(endpoint)
    );
  }
}

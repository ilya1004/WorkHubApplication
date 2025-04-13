import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {HealthCheckResponse} from "../../interfaces/health-checks/health-checks.interface";
import {environment} from "../../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class HealthChecksService {
  private readonly endpoints = [
    `${environment.BASE_URL}projects-service/health`,
    `${environment.BASE_URL}identity-service/health`,
    `${environment.BASE_URL}payments-service/health`,
    `${environment.BASE_URL}chat-service/health`
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

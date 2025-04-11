import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {FreelancerUser} from "../../core/interfaces/freelancer/freelancer-user.interface";
import {EmployerUser} from "../../core/interfaces/employer/employer-user.interface";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(
    private httpClient: HttpClient,
  ) { }

  getFreelancerInfo(userId: string): Observable<FreelancerUser> {
    return this.httpClient.get<FreelancerUser>(
      `${environment.IDENTITY_SERVICE_API_URL}users/freelancer-info/${userId}`
    );
  }

  getEmployerInfo(userId: string): Observable<EmployerUser> {
    return this.httpClient.get<EmployerUser>(
      `${environment.IDENTITY_SERVICE_API_URL}users/employer-info/${userId}`
    );
  }
}

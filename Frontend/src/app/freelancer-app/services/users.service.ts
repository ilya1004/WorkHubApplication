import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {FreelancerUser} from "../../core/interfaces/freelancer/freelancer-user.interface";
import {IDENTITY_SERVICE_API_URL} from "../../core/data/constants";
import {EmployerUser} from "../../core/interfaces/employer/employer-user.interface";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(
    private httpClient: HttpClient,
  ) { }

  getFreelancerInfo(userId: string): Observable<FreelancerUser> {
    return this.httpClient.get<FreelancerUser>(
      `${IDENTITY_SERVICE_API_URL}users/freelancer-info/${userId}`
    );
  }

  getEmployerInfo(userId: string): Observable<EmployerUser> {
    return this.httpClient.get<EmployerUser>(
      `${IDENTITY_SERVICE_API_URL}users/employer-info/${userId}`
    );
  }
}

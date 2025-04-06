import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {FreelancerUserDto} from "../../core/interfaces/freelancer-user-dto.interface";
import {IDENTITY_SERVICE_API_URL} from "../../core/constants";
import {EmployerUserDto} from "../../core/interfaces/employer-user-dto.interface";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(
    private httpClient: HttpClient,
  ) { }

  getFreelancerInfo(userId: string): Observable<FreelancerUserDto> {
    return this.httpClient.get<FreelancerUserDto>(
      `${IDENTITY_SERVICE_API_URL}users/freelancer-info/${userId}`
    );
  }

  getEmployerInfo(userId: string): Observable<EmployerUserDto> {
    return this.httpClient.get<EmployerUserDto>(
      `${IDENTITY_SERVICE_API_URL}users/employer-info/${userId}`
    );
  }
}

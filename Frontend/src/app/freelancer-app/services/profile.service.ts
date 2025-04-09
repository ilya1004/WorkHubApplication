import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {IDENTITY_SERVICE_API_URL} from '../../core/data/constants';
import {Observable} from 'rxjs';
import {PaginatedResult} from '../../core/interfaces/common/paginated-result.interface';
import {FreelancerUser} from "../../core/interfaces/freelancer/freelancer-user.interface";
import {FreelancerSkill} from "../../core/interfaces/freelancer/freelancer-skill.interface";

@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  constructor(
    private httpClient: HttpClient,
    ) { }

  getUserData(): Observable<FreelancerUser> {
    return this.httpClient.get<FreelancerUser>(
      `${IDENTITY_SERVICE_API_URL}users/my-freelancer-info`
    );
  }

  getAvailableSkill(): Observable<PaginatedResult<FreelancerSkill>> {
    return this.httpClient.get<PaginatedResult<FreelancerSkill>>(
      `${IDENTITY_SERVICE_API_URL}freelancer-skills`
    );
  }

  updateFreelancerProfile(formData: FormData): Observable<void> {
    return this.httpClient.put<void>(
      `${IDENTITY_SERVICE_API_URL}users/update-freelancer`,
      formData
    );
  }
  
  changePassword(request: { email: string; currentPassword: string; newPassword: string }): Observable<void> {
    return this.httpClient.post<void>(
      `${IDENTITY_SERVICE_API_URL}users/change-password`,
      request);
  }
}

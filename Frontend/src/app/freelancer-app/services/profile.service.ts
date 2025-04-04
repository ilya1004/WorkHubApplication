import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {IDENTITY_SERVICE_API_URL} from '../../core/constants';
import {Observable} from 'rxjs';
import {FreelancerUser} from '../interfaces/profile/freelancer-user.interface';
import {FreelancerSkill} from '../interfaces/profile/skill.interface';
import {PaginatedResult} from '../../core/interfaces/paginated-result.interface';

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
}

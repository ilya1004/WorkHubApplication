import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {IDENTITY_SERVICE_API_URL} from '../../core/constants';
import {Observable} from 'rxjs';
import {FreelancerProfile} from '../interfaces/profile.interface';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  constructor(
    private httpClient: HttpClient,
    ) { }

  getUserData(): Observable<FreelancerProfile> {
    return this.httpClient.get(`${IDENTITY_SERVICE_API_URL}users/my-info`);
  }
}

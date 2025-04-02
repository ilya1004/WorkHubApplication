import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {IDENTITY_SERVICE_API_URL} from '../../core/constants';
import {Observable} from 'rxjs';
import {FreelancerUser} from '../interfaces/profile.interface';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  constructor(
    private httpClient: HttpClient,
    ) { }

  getUserData(): Observable<FreelancerUser> {
    const token = localStorage.getItem('access_token'); // Получаем токен из локального хранилища
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.httpClient.get<FreelancerUser>(
      `${IDENTITY_SERVICE_API_URL}users/my-info`, { headers }
    );
  }
}

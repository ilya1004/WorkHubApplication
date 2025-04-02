import {Inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {IDENTITY_SERVICE_API_URL} from '../../../core/constants';

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  constructor(
    private httpClient: HttpClient
  ) { }

  login(username: string, password: string) {
    console.log(username, password);
  }
}

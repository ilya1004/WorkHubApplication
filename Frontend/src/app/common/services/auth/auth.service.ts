import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {IDENTITY_SERVICE_API_URL} from '../../../core/constants';
import {AuthInterface} from './auth.interface';
import {tap} from 'rxjs';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  constructor(
    private httpClient: HttpClient
  ) { }


  login(payload: { email: string; password: string }) {
    return this.httpClient.post<AuthInterface>(
      `${IDENTITY_SERVICE_API_URL}auth/login`,
      payload
    ).pipe(
      tap({
        next: val => {
          localStorage.setItem('access_token', val.accessToken);
          localStorage.setItem('refresh_token', val.refreshToken);
        },
        error: err => console.error('Login failed:', err)
      })
    );
  }

  register(payload: { username: string; firstName: string; lastName: string; email: string; password: string }) {
    return this.httpClient.post(
      `${IDENTITY_SERVICE_API_URL}auth/register`, payload
    ).subscribe(value => {
      console.log(value)
    });
  }

  isAuthenticated(): boolean {
    const token = localStorage.getItem('access_token');
    if (!token) return false;

    const decodedToken = this.decodeToken(token);
    return decodedToken && decodedToken.exp * 1000 > Date.now();
  }

  getUserRole(): string | null {
    const token = localStorage.getItem('access_token');
    if (!token) return null;

    const decodedToken = this.decodeToken(token);
    return decodedToken?.role || null;
  }

  private decodeToken(token: string): any {
    try {
      return jwtDecode<{ exp: number; role: string }>(token);
    } catch (error) {
      console.error('Ошибка при декодировании токена:', error);
      return null;
    }
  }
}

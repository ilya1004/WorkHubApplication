import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from "../../../../environments/environment";

const RESET_URL = 'http://localhost:4200/reset-password';

@Injectable({
  providedIn: 'root'
})
export class PasswordResetService {
  constructor(private httpClient: HttpClient) {}

  forgotPassword(email: string): Observable<void> {
    const payload = {
      email,
      resetUrl: RESET_URL
    };
    return this.httpClient.post<void>(`${environment.IDENTITY_SERVICE_API_URL}auth/forgot-password`, payload);
  }

  resetPassword(email: string, newPassword: string, code: string): Observable<void> {
    const payload = {
      email,
      newPassword,
      code
    };
    return this.httpClient.post<void>(`${environment.IDENTITY_SERVICE_API_URL}auth/reset-password`, payload);
  }
}

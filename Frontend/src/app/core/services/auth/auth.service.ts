import {Injectable} from '@angular/core';
import {HttpClient, HttpResponse} from '@angular/common/http';
import {Tokens} from '../../interfaces/auth/tokens';
import {Router} from '@angular/router';
import {Observable, tap} from 'rxjs';
import {TokenService} from "./token.service";
import {environment} from "../../../../environments/environment";

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(
    private httpClient: HttpClient,
    private tokenService: TokenService,
    private router: Router
  ) {}

  login(payload: { email: string; password: string }): Observable<HttpResponse<Tokens>> {
    return this.httpClient.post<Tokens>(
      `${environment.IDENTITY_SERVICE_API_URL}auth/login`,
      payload,
      { observe: 'response' }
    ).pipe(
      tap(response => {
        const authData = response.body;
        if (authData) {
          this.tokenService.setTokens(authData.accessToken, authData.refreshToken);
        }
      })
    );
  }

  logout(): void {
    this.tokenService.clearTokens();
    this.httpClient.post<void>(
      `${environment.IDENTITY_SERVICE_API_URL}/auth/logout`, {})
      .subscribe({
        next: () => {
          this.tokenService.clearTokens();
          this.router.navigate(['/login']);
        },
        error: (error) => {
          console.error('Logout failed:', error);
          this.tokenService.clearTokens();
          this.router.navigate(['/login']);
        }
      });
  }

  registerFreelancer(payload: { userName: string; firstName: string; lastName: string; email: string; password: string }): Observable<HttpResponse<any>> {
    return this.httpClient.post<HttpResponse<any>>(
      `${environment.IDENTITY_SERVICE_API_URL}users/register-freelancer`,
      payload,
      { observe: 'response' }
    );
  }

  registerEmployer(payload: { userName: string; companyName: string; email: string; password: string }): Observable<HttpResponse<any>> {
    return this.httpClient.post<HttpResponse<any>>(
      `${environment.IDENTITY_SERVICE_API_URL}users/register-employer`,
      payload,
      { observe: 'response' }
    );
  }


}
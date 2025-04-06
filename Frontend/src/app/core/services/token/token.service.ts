import { Injectable } from '@angular/core';
import {catchError, map, Observable, of, tap, throwError} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {CookieService} from "ngx-cookie-service";
import {DecodedToken} from "./token.interface";
import {jwtDecode} from "jwt-decode";
import {AuthInterface} from "../auth/auth.interface";
import {IDENTITY_SERVICE_API_URL} from "../../constants";
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root',
})
export class TokenService {

  constructor(
    private httpClient: HttpClient,
    private cookieService: CookieService,
    private router: Router,
  ) {}

  isAuthenticated(): boolean {
    const token = this.getAccessToken();
    if (!token) return false;

    const decodedToken = this.decodeToken(token);
    return decodedToken !== null && decodedToken.exp * 1000 > Date.now();
  }

  getUserRole(): string | null {
    const token = this.getAccessToken();
    if (!token) return null;

    const decodedToken = this.decodeToken(token);
    return decodedToken?.role || null;
  }

  getUserId(): string | null {
    const token = this.getAccessToken();
    if (!token) return null;

    const decoded = this.decodeToken(token);
    return decoded?.userId || null;
  }

  getAccessToken(): string | null {
    const token = this.cookieService.get('access_token');
    if (!token) {
      console.warn('No access token found in cookies');
      return null;
    }

    const decoded = this.decodeToken(token);
    if (!decoded || decoded.exp * 1000 < Date.now()) {
      console.log('Access token is invalid or expired');
      return null;
    }
    return token;
  }

  setTokens(accessToken: string, refreshToken: string): void {
    const expires = this.getTokenExpiration(accessToken);
    this.cookieService.set('access_token', accessToken, { path: '/', expires });
    this.cookieService.set('refresh_token', refreshToken, { path: '/', expires: 30 }); // 30 дней для refresh token
  }

  clearTokens(): void {
    this.cookieService.delete('access_token', '/');
    this.cookieService.delete('refresh_token', '/');
  }

  private decodeToken(token: string): DecodedToken | null {
    try {
      const decoded = jwtDecode<{
        exp: number;
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string;
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': string;
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress': string;
      }>(token);

      return {
        exp: decoded.exp,
        role: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
        userId: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
        email: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
      };
    } catch (error) {
      console.error('Ошибка при декодировании токена:', error);
      return null;
    }
  }

  private getTokenExpiration(token: string): Date | undefined {
    const decoded = this.decodeToken(token);
    return decoded ? new Date(decoded.exp * 1000) : undefined;
  }

  refreshToken(): Observable<string> {

    const refreshToken = this.cookieService.get('refresh_token');
    const accessToken = this.cookieService.get('access_token');

    if (!refreshToken || !accessToken) {
      this.logout();
      return throwError(() => new Error('No refresh or access token available'));
    }

    const payload = {
      accessToken: accessToken,
      refreshToken: refreshToken,
    }

    console.log(payload);

    return this.httpClient.post<AuthInterface>(
      `${IDENTITY_SERVICE_API_URL}auth/refresh-token`, payload
    ).pipe(
      tap(response => {
        this.setTokens(response.accessToken, response.refreshToken);
        console.log('Token refreshed successfully');
      }),
      map(response => response.accessToken),
      catchError(error => {
        this.logout();
        return throwError(() => new Error('Failed to refresh token: ' + error.message));
      })
    );
  }

  ensureValidToken(): Observable<string> {
    const token = this.cookieService.get('access_token');

    if (!token) {
      console.log('No access token found, attempting refresh');
      return this.refreshToken();
    }

    const decoded = this.decodeToken(token);
    if (!decoded || decoded.exp * 1000 < Date.now()) {
      console.log('Access token expired or invalid, attempting refresh');
      return this.refreshToken();
    }

    return of(token);
  }

  logout(): void {
    this.clearTokens();
    this.router.navigate(['/login']);
  }
}
import {Injectable} from '@angular/core';
import {catchError, map, Observable, of, tap, throwError} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {CookieService} from "ngx-cookie-service";
import {DecodedToken} from "../../interfaces/auth/token.interface";
import {jwtDecode} from "jwt-decode";
import {Tokens} from "../../interfaces/auth/tokens";
import {Router} from "@angular/router";
import {environment} from "../../../../environments/environment";

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  private refreshingToken = false;
  private tokenRefreshInProgressLogged = false;
  
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
      if (!this.tokenRefreshInProgressLogged) {
        this.tokenRefreshInProgressLogged = true;
      }
      return null;
    }
    return token;
  }
  
  setTokens(accessToken: string, refreshToken: string): void {
    this.cookieService.set('access_token', accessToken, { path: '/' });
    this.cookieService.set('refresh_token', refreshToken, { path: '/' });
    this.tokenRefreshInProgressLogged = false;
  }
  
  clearTokens(): void {
    this.cookieService.delete('access_token', '/');
    this.cookieService.delete('refresh_token', '/');
    this.tokenRefreshInProgressLogged = false;
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
      console.error('Error in decoding token:', error);
      return null;
    }
  }
  
  private getTokenExpiration(token: string): Date | undefined {
    const decoded = this.decodeToken(token);
    return decoded ? new Date(decoded.exp * 1000) : undefined;
  }
  
  refreshToken(): Observable<string> {
    if (this.refreshingToken) {
      return new Observable(observer => {
        const interval = setInterval(() => {
          if (!this.refreshingToken) {
            clearInterval(interval);
            const token = this.getAccessToken();
            if (token) observer.next(token);
            else observer.error(new Error('Token refresh failed'));
            observer.complete();
          }
        }, 100);
      });
    }
    
    const refreshToken = this.cookieService.get('refresh_token');
    const accessToken = this.cookieService.get('access_token');
    
    if (!refreshToken || !accessToken) {
      this.logout();
      return throwError(() => new Error('No refresh or access token available'));
    }
    
    this.refreshingToken = true;
    const payload = { accessToken, refreshToken };
    
    return this.httpClient.post<Tokens>(
      `${environment.IDENTITY_SERVICE_API_URL}auth/refresh-token`, payload
    ).pipe(
      tap(response => {
        this.setTokens(response.accessToken, response.refreshToken);
        this.refreshingToken = false;
      }),
      map(response => response.accessToken),
      catchError(error => {
        this.refreshingToken = false;
        this.logout();
        return throwError(() => new Error('Failed to refresh token: ' + error.message));
      })
    );
  }
  
  ensureValidToken(): Observable<string> {
    const token = this.getAccessToken();
    if (token) {
      return of(token);
    }
    return this.refreshToken();
  }
  
  logout(): void {
    this.clearTokens();
    this.router.navigate(['/login']);
  }
}
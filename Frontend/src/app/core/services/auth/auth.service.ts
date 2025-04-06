import {Injectable} from '@angular/core';
import {HttpClient, HttpResponse} from '@angular/common/http';
import {IDENTITY_SERVICE_API_URL} from '../../constants';
import {AuthInterface} from './auth.interface';
import {jwtDecode} from 'jwt-decode';
import {CookieService} from 'ngx-cookie-service';
import {Router} from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  constructor(
    private httpClient: HttpClient,
    private cookieService: CookieService,
    private router: Router,
  ) { }


  login(payload: { email: string; password: string }) {
    return this.httpClient.post<AuthInterface>(
      `${IDENTITY_SERVICE_API_URL}auth/login`,
      payload,
      { observe: 'response' }
    );
  }

  logout() {
    this.cookieService.delete('access_token', '/');
    this.cookieService.delete('refresh_token', '/');

    this.router.navigate(['/login']);
  }


  registerFreelancer(payload: { userName: string; firstName: string; lastName: string; email: string; password: string }) {
    return this.httpClient.post<HttpResponse<any>>(
      `${IDENTITY_SERVICE_API_URL}users/register-freelancer`,
      payload,
      { observe: 'response' }
    )
  }

  registerEmployer(payload: { userName: string; companyName: string, email: string; password: string }) {
    return this.httpClient.post(
      `${IDENTITY_SERVICE_API_URL}users/register-employer`,
      payload,
      { observe: 'response' }
    );
  }

  isAuthenticated(): boolean {
    const token = this.cookieService.get('access_token');
    if (!token) return false;

    const decodedToken = this.decodeToken(token);
    if (!decodedToken)
      return false;
    return decodedToken && decodedToken.exp * 1000 > Date.now();
  }

  private decodeToken(token: string): {exp: number, role: string, userId: string, email: string} | null {
    try {
      const decoded = jwtDecode<{
        exp: number;
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": string;
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": string;
      }>(token);

      return {
        exp: decoded.exp,
        role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
        userId: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
        email: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"]
      };
    } catch (error) {
      console.error('Ошибка при декодировании токена:', error);
      return null;
    }
  }

  getUserRole(): string | null {
    if (!this.cookieService.check('access_token')) {
      return null;
    }
    const token = this.cookieService.get('access_token');

    const decodedToken = this.decodeToken(token);
    return decodedToken?.role || null;
  }

  getUserId(): string | null {
    const token = this.getAccessToken();
    if (!token)
      return null;
    const decoded = this.decodeToken(token);
    if (!decoded)
      return null;
    return decoded.userId;
  }

  // getAccessToken(): string | null {
  //   if (!this.cookieService.check('access_token')) {
  //     return null;
  //   }
  //   return this.cookieService.get('access_token');
  // }

  getAccessToken(): string | null {
    const token = this.cookieService.get('access_token');
    if (!token) {
      console.warn('No access token found in cookies');
      return null;
    }

    const decoded = this.decodeToken(token);
    if (decoded && decoded.exp * 1000 < Date.now()) {
      console.log('Token expired, attempting refresh');
      // this.refreshToken();
      return this.cookieService.get('access_token');
    }
    return token;
  }
}

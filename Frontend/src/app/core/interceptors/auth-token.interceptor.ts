import {HttpEvent, HttpHandlerFn, HttpInterceptorFn, HttpRequest} from '@angular/common/http';
import {inject} from '@angular/core';
import {catchError, Observable, switchMap, throwError} from "rxjs";
import {TokenService} from "../services/auth/token.service";
import {Router} from "@angular/router";

export const authTokenInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> => {
  const tokenService = inject(TokenService);
  const router = inject(Router);
  
  if (req.url.includes('login') ||
    req.url.includes('register') ||
    req.url.includes('refresh-token') ||
    req.url.includes('resend-email-confirmation') ||
    req.url.includes('confirm-email') ||
    req.url.includes('forgot-password') ||
    req.url.includes('reset-password')) {
    return next(req);
  }
  
  return tokenService.ensureValidToken().pipe(
    switchMap(token => {
      if (!token) {
        console.warn('No valid token available, redirecting to login');
        router.navigate(['/login']);
        return throwError(() => new Error('No valid token available'));
      }
      
      const authReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
      return next(authReq);
    }),
    catchError(error => {
      if (error.status === 401) {
        console.error('Unauthorized request, attempting refresh');
        return tokenService.refreshToken().pipe(
          switchMap(newToken => {
            const authReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${newToken}`
              }
            });
            return next(authReq);
          }),
          catchError(refreshError => {
            console.error('Refresh failed, redirecting to login:', refreshError);
            tokenService.clearTokens();
            router.navigate(['/login']);
            return throwError(() => refreshError);
          })
        );
      }
      if (error.status === 403) {
        console.error('Forbidden request, redirecting to login');
        tokenService.clearTokens();
        router.navigate(['/login']);
      }
      return throwError(() => error);
    })
  );
};
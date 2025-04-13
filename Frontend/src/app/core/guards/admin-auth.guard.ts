import {catchError, Observable, of, switchMap} from "rxjs";
import {inject} from "@angular/core";
import {TokenService} from "../services/auth/token.service";
import {Router} from "@angular/router";

export const canActivateAdminApp = (): Observable<boolean> | boolean => {
  const tokenService = inject(TokenService);
  const router = inject(Router);
  
  const isAuthenticated = tokenService.isAuthenticated();
  const role = tokenService.getUserRole();
  
  if (isAuthenticated && role === 'Admin') {
    return true;
  }
  
  return tokenService.ensureValidToken().pipe(
    switchMap(() => {
      const updatedRole = tokenService.getUserRole();
      if (updatedRole === 'Admin') {
        return of(true);
      } else {
        console.warn('User role is not Admin after token refresh, redirecting to login');
        router.navigate(['/login']);
        return of(false);
      }
    }),
    catchError((error) => {
      console.warn('Token refresh failed, redirecting to login:', error);
      router.navigate(['/login']);
      return of(false);
    })
  );
}
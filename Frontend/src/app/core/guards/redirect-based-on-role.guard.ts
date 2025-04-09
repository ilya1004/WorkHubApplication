import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';
import {TokenService} from "../services/token/token.service";

export const redirectBasedOnRole = (): Observable<boolean> => {
  const tokenService = inject(TokenService);
  const router = inject(Router);
  
  const isAuthenticated = tokenService.isAuthenticated();
  const role = tokenService.getUserRole();
  
  if (isAuthenticated && role) {
    switch (role) {
      case 'Freelancer':
        router.navigate(['/freelancer/home']);
        break;
      case 'Employer':
        router.navigate(['/employer/home']);
        break;
      case 'Admin':
        router.navigate(['/admin/home']);
        break;
      default:
        router.navigate(['/login']); // Если роль неизвестна, редирект на логин
    }
    return of(false);
  }
  
  return tokenService.ensureValidToken().pipe(
    switchMap(() => {
      const updatedRole = tokenService.getUserRole();
      switch (updatedRole) {
        case 'Freelancer':
          router.navigate(['/freelancer/home']);
          break;
        case 'Employer':
          router.navigate(['/employer/home']);
          break;
        case 'Admin':
          router.navigate(['/admin/home']);
          break;
        default:
          router.navigate(['/login']);
      }
      return of(false);
    }),
    catchError(() => {
      console.warn('Token refresh failed, redirecting to login');
      router.navigate(['/login']);
      return of(false);
    })
  );
};
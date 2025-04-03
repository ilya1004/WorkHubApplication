import {inject} from '@angular/core';
import {AuthService} from './auth.service';
import {Router} from '@angular/router';

export const  canActivateFreelancerAuth = () => {
  const authService = inject(AuthService);

  const isAuthenticated = authService.isAuthenticated();
  const role = authService.getUserRole();

  if (isAuthenticated && role == 'Freelancer') {
    return true;
  }

  return inject(Router).createUrlTree(['/login']);
}

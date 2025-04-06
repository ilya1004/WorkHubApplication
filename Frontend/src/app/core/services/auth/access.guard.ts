import {inject} from '@angular/core';
import {AuthService} from './auth.service';
import {Router} from '@angular/router';
import {TokenService} from "../token/token.service";

export const  canActivateFreelancerAuth = () => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  const isAuthenticated = tokenService.isAuthenticated();
  const role = tokenService.getUserRole();

  if (isAuthenticated && role === 'Freelancer') {
    return true;
  }

  console.warn('User is not authenticated, redirecting to login');
  router.navigate(['/login']);
  return false;
}

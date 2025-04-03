import { HttpInterceptorFn } from '@angular/common/http';
import {AuthService} from '../services/auth/auth.service';
import {inject} from '@angular/core';

export const authTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const accessToken = inject(AuthService).getAccessToken();
  if (!accessToken) {
    return next(req);
  }

  req = req.clone({
    setHeaders: {
      Authorization: `Bearer ${accessToken}`
    }
  });

  return next(req);
};

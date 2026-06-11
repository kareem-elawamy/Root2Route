import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { ToastService } from '../services/toast.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getToken();

  if (!token || req.headers.has('Authorization')) {
    return next(req);
  }

  const authReq = req.clone({
    setHeaders: { Authorization: `Bearer ${token}` },
  });

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      const toast = inject(ToastService);
      const router = inject(Router);

      if (error.status === 401) {
        toast.error('Session expired. Please log in again.');
        auth.clearSession();
        router.navigate(['/login']);
      } else if (error.status === 403) {
        toast.error('You do not have permission to perform this action.');
      } else if (error.status === 404) {
        toast.warning('Resource not found.');
      } else if (error.status >= 500) {
        toast.error('Server error. Please try again later.');
      } else if (error.status === 0) {
        toast.error('Network error. Check your connection.');
      }

      return throwError(() => error);
    })
  );
};

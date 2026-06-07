import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (): boolean | UrlTree => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (!auth.isAuthenticated()) {
    return router.createUrlTree(['/auth/login']);
  }

  // If authenticated, check role for routing
  if (auth.isSuperAdmin()) {
    return router.createUrlTree(['/admin-dashboard']);
  }

  // We assume that if they aren't SuperAdmin, they belong to the company logic
  return router.createUrlTree(['/company-dashboard']);
};

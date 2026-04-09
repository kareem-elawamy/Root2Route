import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const permissionGuard = (requiredPermission: string): CanActivateFn => {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (auth.hasPermission(requiredPermission)) {
      return true;
    }
    return router.createUrlTree(['/unauthorized']);
  };
};

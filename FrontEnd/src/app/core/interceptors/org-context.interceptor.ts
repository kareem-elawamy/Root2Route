import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { OrgContextService } from '../services/org-context.service';

export const orgContextInterceptor: HttpInterceptorFn = (req, next) => {
  const orgContext = inject(OrgContextService);
  const orgId = orgContext.getActiveOrgId();

  if (!orgId) {
    return next(req);
  }

  const orgReq = req.clone({
    setHeaders: { 'X-Organization-Id': orgId },
  });

  return next(orgReq);
};

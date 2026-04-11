import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { noAuthGuard } from './core/guards/no-auth.guard';
import { OrgLayoutComponent } from './layouts/org-layout-component/org-layout-component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'auth/login',
    pathMatch: 'full',
  },
  {
    path: 'auth',
    canActivate: [noAuthGuard],
    loadChildren: () =>
      import('./features/auth/auth.routes').then((m) => m.authRoutes),
  },

  {
    path: 'super-admin',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./layouts/super-admin-layout/super-admin-layout.component').then(
        (m) => m.SuperAdminLayoutComponent
      ),
    loadChildren: () =>
      import('./features/super-admin/super-admin.routes').then(
        (m) => m.superAdminRoutes
      ),
  },

  {
    path: 'org',
    canActivate: [authGuard],
    component: OrgLayoutComponent,
    loadChildren: () =>
      import('./features/organization/organization.routes').then(
        (m) => m.organizationRoutes
      ),
  },

  {
    path: 'unauthorized',
    loadComponent: () =>
      import('./shared/components/unauthorized/unauthorized.component').then(
        (m) => m.UnauthorizedComponent
      ),
  },

  {
    path: '**',
    loadComponent: () =>
      import('./shared/components/not-found/not-found.component').then(
        (m) => m.NotFoundComponent
      ),
  },
];

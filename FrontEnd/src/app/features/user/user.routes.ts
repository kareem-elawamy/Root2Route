import { Routes } from '@angular/router';

export const userRoutes: Routes = [
  {
    path: '',
    redirectTo: 'invitations',
    pathMatch: 'full',
  },
  {
    path: 'invitations',
    loadComponent: () =>
      import('./pages/my-invitations/my-invitations').then((m) => m.MyInvitationsComponent),
  },
  {
    path: 'change-password',
    loadComponent: () =>
      import('./pages/change-password/change-password').then((m) => m.ChangePasswordComponent),
  },
  {
    path: 'account-settings',
    loadComponent: () =>
      import('./pages/account-settings/account-settings').then((m) => m.AccountSettingsComponent),
  },
];

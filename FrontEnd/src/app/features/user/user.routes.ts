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
  }
];

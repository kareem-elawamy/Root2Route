import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/loginComponent';
// import { MagicLinkComponent } from './pages/magic-link/magic-link.component';

export const authRoutes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
  },
  // {
  //   path: 'magic-link',
  //   component: MagicLinkComponent,
  // },
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
];

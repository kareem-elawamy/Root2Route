import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { MainLayout } from './layouts/super-admin-layout/main-layout';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: Login },

  {
    path: '',
    component: MainLayout,
    loadChildren: () => import('./features/super-admin/super-admin.routes').then(m => m.SUPER_ADMIN_ROUTES)
  },

  { path: '**', redirectTo: '/login' }
];
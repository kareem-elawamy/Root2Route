import { Routes } from '@angular/router';
import { SuperAdminLayoutComponent } from '../../layouts/super-admin-layout/super-admin-layout.component';

import { Dashboard } from './dashboard/dashboard';
import { Organizations } from './organizations/organizations';
import { Auctions } from './auctions/auctions';
import { AiLab } from './ai-lab/ai-lab';
import { Reports } from './reports/reports';
import { Settings } from './settings/settings';
import { Products } from './products/products';

export const superAdminRoutes: Routes = [
  {
    path: '',
    component: SuperAdminLayoutComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: Dashboard },
      { path: 'organizations', component: Organizations },
      { path: 'products', component: Products },
      { path: 'auctions', component: Auctions },
      { path: 'ai-lab', component: AiLab },
      { path: 'reports', component: Reports },
      { path: 'settings', component: Settings },

      // New pages (Phase 4)
      {
        path: 'users',
        loadComponent: () => import('./users/users').then(m => m.Users)
      },
      {
        path: 'transactions',
        loadComponent: () => import('./transactions/transactions').then(m => m.Transactions)
      },
      {
        path: 'audit-log',
        loadComponent: () => import('./audit-log/audit-log').then(m => m.AuditLog)
      },
    ]
  }
];
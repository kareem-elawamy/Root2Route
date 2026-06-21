import { Routes } from '@angular/router';
import { SuperAdminLayoutComponent } from '../../layouts/super-admin-layout/super-admin-layout.component';

import { Dashboard } from './dashboard/dashboard';
import { Organizations } from './organizations/organizations';
import { Auctions } from './auctions/auctions';
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
      { path: 'reports', component: Reports },
      { path: 'settings', component: Settings },
      { 
        path: 'plants', 
        loadComponent: () => import('./plants/plants').then(m => m.Plants) 
      },

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
        path: 'withdrawals',
        loadComponent: () => import('./withdrawals/withdrawals').then(m => m.Withdrawals)
      },
      {
        path: 'audit-log',
        loadComponent: () => import('./audit-log/audit-log').then(m => m.AuditLog)
      },
    ]
  }
];
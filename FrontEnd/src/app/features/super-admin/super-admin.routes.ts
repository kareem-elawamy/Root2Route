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
    component: SuperAdminLayoutComponent, // 🟢 خلينا السايد بار هنا هو الأب
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: Dashboard },
      { path: 'organizations', component: Organizations },
      { path: 'auctions', component: Auctions },
      { path: 'products', component: Products },
      { path: 'ai-lab', component: AiLab },
      { path: 'reports', component: Reports },
      { path: 'settings', component: Settings }
    ]
  }
];
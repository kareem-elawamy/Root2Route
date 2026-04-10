import { Routes } from '@angular/router';
import { Dashboard } from './dashboard/dashboard';
import { Organizations } from './organizations/organizations';
import { Auctions } from './auctions/auctions';
import { AiLab } from './ai-lab/ai-lab';
import { Reports } from './reports/reports';
import { Settings } from './settings/settings';

export const SUPER_ADMIN_ROUTES: Routes = [
  { path: 'dashboard', component: Dashboard },
  { path: 'organizations', component: Organizations },
  { path: 'auctions', component: Auctions },
  { path: 'ai-lab', component: AiLab },
  { path: 'reports', component: Reports },
  { path: 'settings', component: Settings }
];

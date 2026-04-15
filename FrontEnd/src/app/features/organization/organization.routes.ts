import { Routes } from '@angular/router';
import { OverviewComponent } from './pages/overview-component/overview-component';
import { ProductsComponent } from './pages/products-component/products-component';

export const organizationRoutes: Routes = [
  {
    path: '',
    redirectTo: 'overview',
    pathMatch: 'full',
  },
  {
    path: 'overview',
    component: OverviewComponent
  },
  // {
  //   path: 'auctions',
  //   loadComponent: () =>
  //     import('./pages/auctions/auctions.component').then(
  //       (m) => m.AuctionsComponent
  //     ),
  // },
  {
    path: 'products',
    component: ProductsComponent
  }
  // {
  //   path: 'orders',
  //   loadComponent: () =>
  //     import('./pages/orders/orders.component').then((m) => m.OrdersComponent),
  // },
  // {
  //   path: 'members',
  //   loadComponent: () =>
  //     import('./pages/members/members.component').then(
  //       (m) => m.MembersComponent
  //     ),
  // },
  // {
  //   path: 'settings',
  //   loadComponent: () =>
  //     import('./pages/settings/settings.component').then(
  //       (m) => m.SettingsComponent
  //     ),
  //  },
];

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
  {
    path: 'auctions',
    loadComponent: () =>
      import('./pages/auctions/auctions').then(
        (m) => m.AuctionsComponent
      ),
  },
  {
    path: 'products',
    component: ProductsComponent
  },
  {
    path: 'orders',
    loadComponent: () =>
      import('./pages/orders/orders').then((m) => m.OrdersComponent),
  },
  {
    path: 'members',
    loadComponent: () =>
      import('./pages/members/members').then((m) => m.MembersComponent),
  },
  {
    path: 'chat',
    loadComponent: () =>
      import('./pages/chat/chat').then((m) => m.ChatComponent),
  },
  {
    path: 'reviews',
    loadComponent: () =>
      import('./pages/reviews/reviews').then((m) => m.ReviewsComponent),
  },
  {
    path: 'shipments',
    loadComponent: () =>
      import('./pages/shipments/shipments').then((m) => m.ShipmentsComponent),
  },
  // {
  //   path: 'settings',
  //   loadComponent: () =>
  //     import('./pages/settings/settings.component').then(
  //       (m) => m.SettingsComponent
  //     ),
  //  },
];

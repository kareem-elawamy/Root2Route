import { Routes } from '@angular/router';
import { OverviewComponent } from './pages/overview-component/overview-component';
import { ProductsComponent } from './pages/products-component/products-component';
import { permissionGuard } from '../../core/guards/permission.guard';

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
    canActivate: [permissionGuard('Permissions.Auctions.View')],
    loadComponent: () =>
      import('./pages/auctions/auctions').then(
        (m) => m.AuctionsComponent
      ),
  },
  {
    path: 'products',
    canActivate: [permissionGuard('Permissions.Market.ViewProducts')],
    component: ProductsComponent
  },
  {
    path: 'orders',
    loadComponent: () =>
      import('./pages/orders/orders').then((m) => m.OrdersComponent),
  },
  {
    path: 'members',
    canActivate: [permissionGuard('Permissions.Members.View')],
    loadComponent: () =>
      import('./pages/members/members').then((m) => m.MembersComponent),
  },
  {
    path: 'roles',
    canActivate: [permissionGuard('Permissions.Roles.View')],
    loadComponent: () =>
      import('./pages/roles/roles').then((m) => m.RolesComponent),
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
  {
    path: 'settings',
    canActivate: [permissionGuard('Permissions.Organization.ManageSettings')],
    loadComponent: () =>
      import('./pages/settings/settings').then(
        (m) => m.SettingsComponent
      ),
   },
];

import { Routes, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { OverviewComponent } from './pages/overview-component/overview-component';
import { ProductsComponent } from './pages/products-component/products-component';
import { permissionGuard } from '../../core/guards/permission.guard';

export const organizationRoutes: Routes = [
  {
    path: '',
    canActivate: [
      () => {
        const auth = inject(AuthService);
        const router = inject(Router);

        if (auth.hasPermission('Permissions.Organization.View')) {
          return router.createUrlTree(['/company-dashboard/overview']);
        }
        if (auth.hasPermission('Permissions.Market.ViewProducts')) {
          return router.createUrlTree(['/company-dashboard/products']);
        }
        if (auth.hasPermission('Permissions.Auctions.View')) {
          return router.createUrlTree(['/company-dashboard/auctions']);
        }
        if (auth.hasPermission('Permissions.Members.View')) {
          return router.createUrlTree(['/company-dashboard/members']);
        }
        if (auth.hasPermission('Permissions.Roles.View')) {
          return router.createUrlTree(['/company-dashboard/roles']);
        }
        return router.createUrlTree(['/user/invitations']);
      }
    ],
    pathMatch: 'full',
    component: OverviewComponent
  },
  {
    path: 'overview',
    canActivate: [permissionGuard('Permissions.Organization.View')],
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
    canActivate: [permissionGuard('Permissions.Market.ViewProducts')],
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
    canActivate: [permissionGuard('Permissions.Market.ViewProducts')],
    loadComponent: () =>
      import('./pages/chat/chat').then((m) => m.ChatComponent),
  },
  {
    path: 'reviews',
    canActivate: [permissionGuard('Permissions.Organization.ManageSettings')],
    loadComponent: () =>
      import('./pages/reviews/reviews').then((m) => m.ReviewsComponent),
  },
  {
    path: 'shipments',
    canActivate: [permissionGuard('Permissions.Organization.ManageSettings')],
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
   {
     path: 'plant-analysis',
     loadComponent: () =>
       import('./pages/plant-analysis/plant-analysis').then(
         (m) => m.PlantAnalysisComponent
       ),
   },
];

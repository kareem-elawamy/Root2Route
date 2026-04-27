import { Routes } from '@angular/router';
import { OverviewComponent } from './pages/overview-component/overview-component';
import { ProductsComponent } from './pages/products-component/products-component';
import { OrdersComponent } from './pages/orders-component/orders-component';
import { AuctionsComponent } from './pages/auctions-component/auctions-component';
import { MembersComponent } from './pages/members-component/members-component';
import { ChatComponent } from './pages/chat-component/chat-component';
import { ShipmentsComponent } from './pages/shipments-component/shipments-component';

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
    component: AuctionsComponent
  },
  {
    path: 'products',
    component: ProductsComponent
  },
  {
    path: 'orders',
    component: OrdersComponent
  },
  {
    path: 'members',
    component: MembersComponent
  },
  {
    path: 'chat',
    component: ChatComponent
  },
  {
    path: 'shipments',
    component: ShipmentsComponent
  }
  // {
  //   path: 'settings',
  //   loadComponent: () =>
  //     import('./pages/settings/settings.component').then(
  //       (m) => m.SettingsComponent
  //     ),
  //  },
];

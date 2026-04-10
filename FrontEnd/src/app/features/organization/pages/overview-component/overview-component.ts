import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { AuthService } from '../../../../core/services/auth.service';


@Component({
  selector: 'app-overview',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './overview-component.html',
  styleUrl: './overview-component.css',
})
export class OverviewComponent implements OnInit {
  private readonly orgCtx = inject(OrgContextService);
  private readonly auth = inject(AuthService);

  readonly activeOrg = this.orgCtx.activeOrg;
  readonly currentUser = this.auth.currentUser;
  readonly overview = this.orgCtx.overview();
  metrics = signal<any[]>([]);

  ngOnInit(): void {
    this.overview.subscribe((overview) => {
      this.metrics.set([
        { title: 'Total Revenue', value: '$' + overview.totalRevenue, trend: '+14%', isUp: true, icon: 'payments' },
        { title: 'Active Auctions', value: overview.activeAuctions, trend: '+5%', isUp: true, icon: 'gavel' },
        { title: 'Pending Orders', value: overview.pendingOrders, trend: '-2%', isUp: false, icon: 'local_shipping' },
        { title: 'Unread Messages', value: overview.unreadMessages, trend: 'New', isUp: true, icon: 'forum' },
      ]);
    });
  }



  recentOrders = [
    { id: '#ORD-0091', product: 'Premium Arabica Coffee Beans', status: 'Processing', amount: '$1,200' },
    { id: '#ORD-0090', product: 'Organic Fertilizer (Bulk)', status: 'Shipped', amount: '$450' },
    { id: '#ORD-0089', product: 'Irrigation Sensors', status: 'Delivered', amount: '$890' },
    { id: '#ORD-0088', product: 'Heirloom Tomato Seeds', status: 'Processing', amount: '$120' },
  ];

  liveBids = [
    { auction: 'Wheat Harvest 2024 (10 Tons)', bidder: 'AgriCorp', amount: '$2,800', time: '2 mins ago' },
    { auction: 'Used John Deere Tractor', bidder: 'Farm Solutions Ltd', amount: '$45,000', time: '15 mins ago' },
    { auction: 'Organic Honey (500 liters)', bidder: 'Sweet Naturals', amount: '$3,100', time: '1 hour ago' },
  ];
}

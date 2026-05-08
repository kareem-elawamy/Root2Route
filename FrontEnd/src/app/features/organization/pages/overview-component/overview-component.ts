import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { AuthService } from '../../../../core/services/auth.service';
import { LatestOrder } from '../../../../core/model/Organization/latestOrder';
import { LiveBid } from '../../../../core/model/Organization/liveBid';
import { ActivityChartItem } from '../../../../core/model/Organization/activityChartItem';


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
  private readonly router = inject(Router);
  readonly activeOrg = this.orgCtx.activeOrg;
  readonly currentUser = this.auth.currentUser;

  metrics = signal<any[]>([]);
  recentOrders = signal<LatestOrder[]>([]);
  liveBids = signal<LiveBid[]>([]);
  chartData = signal<ActivityChartItem[]>([]);

  ngOnInit(): void {
    if (!this.auth.isLogin()) {
      this.router.navigate(['/login']);
    }

    // 1. KPI Metrics
    this.orgCtx.overview().subscribe({
      next: (overview) => {
        this.metrics.set([
          { title: 'Total Revenue', value: '$' + overview.totalRevenue.toLocaleString(), trend: '+14%', isUp: true, icon: 'payments' },
          { title: 'Active Auctions', value: overview.activeAuctions, trend: '+5%', isUp: true, icon: 'gavel' },
          { title: 'Pending Orders', value: overview.pendingOrders, trend: '-2%', isUp: false, icon: 'local_shipping' },
          { title: 'Unread Messages', value: overview.unreadMessages, trend: 'New', isUp: true, icon: 'forum' },
        ]);
      }
    });

    // 2. Recent Orders
    this.orgCtx.latestOrders(5).subscribe({
      next: (orders) => this.recentOrders.set(orders)
    });

    // 3. Live Feed
    this.orgCtx.liveBids(10).subscribe({
      next: (bids) => this.liveBids.set(bids)
    });

    // 4. Chart Data
    this.orgCtx.activityChart(6).subscribe({
      next: (data) => this.chartData.set(data)
    });
  }

  getStatusLabel(status: number): string {
    const statuses: { [key: number]: string } = {
      0: 'Pending',
      1: 'Processing',
      2: 'Shipped',
      3: 'Delivered',
      4: 'Cancelled',
      5: 'Refunded'
    };
    return statuses[status] || 'Unknown';
  }

  formatTime(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);

    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins}m ago`;
    const diffHours = Math.floor(diffMins / 60);
    if (diffHours < 24) return `${diffHours}h ago`;
    return date.toLocaleDateString();
  }
}

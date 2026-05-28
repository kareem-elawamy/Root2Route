import { Component, inject, OnInit, signal, ChangeDetectorRef, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { AuthService } from '../../../../core/services/auth.service';
import { DashboardService } from '../../dashboard.service';

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
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly dashboardService = inject(DashboardService);

  readonly activeOrg = this.orgCtx.activeOrg;
  readonly currentUser = this.auth.currentUser;
  metrics = signal<any[]>([]);
  chartData = signal<any[]>([
    { month: 'Jan', netRevenue: 12000 },
    { month: 'Feb', netRevenue: 15000 },
    { month: 'Mar', netRevenue: 18000 },
    { month: 'Apr', netRevenue: 14000 },
    { month: 'May', netRevenue: 22000 },
    { month: 'Jun', netRevenue: 26000 },
    { month: 'Jul', netRevenue: 30000 }
  ]);

  recentOrders = signal<any[]>([]);
  liveBids = signal<any[]>([]);

  constructor() {
    effect(() => {
      const org = this.activeOrg();
      if (org && org.id) {
        this.loadDashboardData(org.id);
      }
    });
  }

  ngOnInit(): void {
    if (!this.auth.isLogin()) {
      this.router.navigate(['/login']);
      return;
    }
  }

  loadDashboardData(orgId: string) {
    // Load Overview Stats
    this.dashboardService.getOverview(orgId).subscribe({
      next: (response: any) => {
        const data = response?.data || response || {};
        this.metrics.set([
          { title: 'Total Revenue', value: '$' + (data.totalRevenue || 0), trend: data.revenueTrend || '+0%', isUp: true, icon: 'payments' },
          { title: 'Active Auctions', value: data.activeAuctions || 0, trend: data.auctionsTrend || '+0%', isUp: true, icon: 'gavel' },
          { title: 'Pending Orders', value: data.pendingOrders || 0, trend: data.ordersTrend || '-0%', isUp: false, icon: 'local_shipping' },
          { title: 'Unread Messages', value: data.unreadMessages || 0, trend: 'New', isUp: true, icon: 'forum' },
        ]);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching overview stats', err);
      }
    });

    // Load Latest Orders
    this.dashboardService.getLatestOrders(orgId).subscribe({
      next: (response: any) => {
        const data = response?.data || response || [];
        this.recentOrders.set(Array.isArray(data) ? data : []);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching latest orders', err);
      }
    });

    // Load Live Bids
    this.dashboardService.getLiveBids(orgId).subscribe({
      next: (response: any) => {
        const data = response?.data || response || [];
        this.liveBids.set(Array.isArray(data) ? data : []);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching live bids', err);
      }
    });
  }

  getStatusLabel(status: number): string {
    switch (status) {
      case 1: return 'Pending';
      case 2: return 'Processing';
      case 3: return 'Completed';
      case 4: return 'Cancelled';
      default: return 'Unknown';
    }
  }

  formatTime(dateStr: string): string {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }
}

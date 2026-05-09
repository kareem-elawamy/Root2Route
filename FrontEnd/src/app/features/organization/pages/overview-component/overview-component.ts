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

  recentOrders: any[] = [];
  liveBids: any[] = [];

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
        this.recentOrders = Array.isArray(data) ? data : [];
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
        this.liveBids = Array.isArray(data) ? data : [];
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching live bids', err);
      }
    });
  }
}
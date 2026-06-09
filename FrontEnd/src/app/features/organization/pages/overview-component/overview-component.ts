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
  showChartOptions = signal(false);
  activeTimeframe = signal(6); // default to 6 months
  selectedDateRange = signal('Last 30 Days');
  isDateDropdownOpen = signal(false);
  isHelpOpen = signal(false);

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
    this.loadOverviewStats(orgId);

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

    this.loadChartData(orgId, this.activeTimeframe());
  }

  loadOverviewStats(orgId: string, days?: number) {
    this.dashboardService.getOverview(orgId, days).subscribe({
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
  }

  loadChartData(orgId: string, months: number) {
    this.dashboardService.getActivityChart(orgId, months).subscribe({
      next: (response: any) => {
        const data = response?.data || response || [];
        if (Array.isArray(data) && data.length > 0) {
           this.chartData.set(data);
        }
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error fetching chart', err)
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

  actionPlaceholder(actionName: string): void {
    alert(`Feature '${actionName}' is coming soon!`);
  }

  toggleChartOptions(): void {
    this.showChartOptions.update(v => !v);
  }

  toggleDateDropdown(): void {
    this.isDateDropdownOpen.update(v => !v);
  }

  onDateRangeChange(range: string): void {
    this.selectedDateRange.set(range);
    this.isDateDropdownOpen.set(false);
    console.log(`[Data Fetch] Updating KPI cards based on range: ${range}`);
    
    // Simulate updating chart/metrics data timeframe
    let days = 30;
    if (range === 'Today') days = 1;
    else if (range === 'Last 7 Days') days = 7;
    else if (range === 'This Year') days = 365;
    
    this.setFilter(days);
  }

  setFilter(days: number): void {
    const org = this.activeOrg();
    if (org?.id) {
      this.loadOverviewStats(org.id, days);
      this.activeTimeframe.set(days === 30 ? 1 : 6); // Just keeping the existing simulation logic
      this.loadChartData(org.id, this.activeTimeframe());
    }
  }

  toggleHelpPopover(): void {
    this.isHelpOpen.update(v => !v);
  }

  closeHelpPopover(): void {
    this.isHelpOpen.set(false);
  }

  exportReport(): void {
    const orders = this.recentOrders();
    if (!orders || orders.length === 0) {
      alert("No data to export.");
      return;
    }
    
    // Create CSV content
    const headers = ["Order ID", "Product", "Amount", "Status"];
    const rows = orders.map(o => [
      o.orderId,
      `"${o.buyerName}"`,
      o.totalAmount,
      this.getStatusLabel(o.status)
    ]);
    
    const csvContent = "data:text/csv;charset=utf-8," 
      + [headers.join(","), ...rows.map(e => e.join(","))].join("\n");
      
    // Download
    const encodedUri = encodeURI(csvContent);
    const link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", `organization_report_${new Date().getTime()}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}

import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaymentService } from '../../../core/services/payment.service';
import { PaytabsService } from '../../../core/services/paytabs.service';
import { ToastService } from '../../../core/services/toast.service';
import { SkeletonComponent } from '../../../shared/components/skeleton/skeleton.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { StatChartComponent, ChartDataset } from '../../../shared/components/stat-chart/stat-chart.component';

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [CommonModule, FormsModule, SkeletonComponent, PaginationComponent, StatChartComponent],
  templateUrl: './transactions.html',
  styleUrl: './transactions.css'
})
export class Transactions implements OnInit {
  private paymentService = inject(PaymentService);
  private paytabsService = inject(PaytabsService);
  private toast = inject(ToastService);

  // Tabs state
  activeTab = signal<'ledger' | 'analytics'>('ledger');

  // Ledger state
  payments = signal<any[]>([]);
  isLoading = signal(true);
  dateFrom = signal('');
  dateTo = signal('');
  filterStatus = signal('');
  currentPage = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);
  pageSize = 15;

  stats = signal({ totalVolume: 0, platformFees: 0, completed: 0, pending: 0 });

  // Analytics state
  isAnalyticsLoading = signal(false);
  analyticsFrom = signal('');
  analyticsTo = signal('');
  analyticsData = signal<any>(null);

  // Computeds for PayTabs analytics charts
  chartLabels = computed(() => {
    const data = this.analyticsData();
    if (!data || !data.dailyBreakdown) return [];
    return data.dailyBreakdown.map((item: any) => item.date);
  });

  chartDatasets = computed((): ChartDataset[] => {
    const data = this.analyticsData();
    if (!data || !data.dailyBreakdown) return [];
    
    // Revenue line dataset
    return [
      {
        label: 'Gross Volume (EGP)',
        data: data.dailyBreakdown.map((item: any) => item.revenue || 0),
        borderColor: '#34d399',
        backgroundColor: 'rgba(52, 211, 153, 0.08)',
        fill: true,
        tension: 0.4
      }
    ];
  });

  methodChartLabels = computed(() => {
    const data = this.analyticsData();
    if (!data || !data.paymentMethodBreakdown) return [];
    return data.paymentMethodBreakdown.map((item: any) => item.method || 'Unknown');
  });

  methodChartDatasets = computed((): ChartDataset[] => {
    const data = this.analyticsData();
    if (!data || !data.paymentMethodBreakdown) return [];
    
    return [
      {
        label: 'Transactions Count',
        data: data.paymentMethodBreakdown.map((item: any) => item.count || 0),
        backgroundColor: [
          'rgba(99, 102, 241, 0.6)', // Indigo
          'rgba(52, 211, 153, 0.6)', // Green
          'rgba(251, 191, 36, 0.6)', // Yellow
          'rgba(239, 68, 68, 0.6)'    // Red
        ],
        borderColor: [
          '#6366f1',
          '#34d399',
          '#fbbf24',
          '#ef4444'
        ],
        borderWidth: 1
      }
    ];
  });

  ngOnInit() {
    this.loadPayments();
    
    // Set default dates for analytics: past 30 days
    const today = new Date();
    const past30Days = new Date();
    past30Days.setDate(today.getDate() - 30);
    
    this.analyticsTo.set(today.toISOString().slice(0, 10));
    this.analyticsFrom.set(past30Days.toISOString().slice(0, 10));
    this.loadAnalytics();
  }

  loadPayments() {
    this.isLoading.set(true);
    this.paymentService.getAllPayments(
      this.currentPage(),
      this.pageSize,
      this.dateFrom() || undefined,
      this.dateTo() || undefined,
      this.filterStatus() || undefined
    ).subscribe({
      next: (response: any) => {
        const data = response.data || response;
        const items = data.data || data.items || data.payments || (Array.isArray(data) ? data : []);
        this.payments.set(items);
        this.totalCount.set(data.totalCount || items.length);
        this.totalPages.set(Math.ceil(this.totalCount() / this.pageSize) || 1);

        const volume = items.reduce((s: number, p: any) => s + (p.amount || 0), 0);
        this.stats.set({
          totalVolume: volume,
          platformFees: volume * 0.05,
          completed: items.filter((p: any) => p.paymentStatus === 'Completed' || p.paymentStatus === 1).length,
          pending: items.filter((p: any) => p.paymentStatus === 'Pending' || p.paymentStatus === 0).length
        });

        this.isLoading.set(false);
      },
      error: (err: any) => {
        console.error('Error loading payments', err);
        this.isLoading.set(false);
      }
    });
  }

  loadAnalytics() {
    if (!this.analyticsFrom() || !this.analyticsTo()) return;

    this.isAnalyticsLoading.set(true);
    
    // Convert to ISO-8601 start/end of day string representation
    const fromDate = new Date(this.analyticsFrom());
    fromDate.setHours(0, 0, 0, 0);
    const toDate = new Date(this.analyticsTo());
    toDate.setHours(23, 59, 59, 999);

    this.paytabsService.getSuperAdminAnalytics(fromDate.toISOString(), toDate.toISOString()).subscribe({
      next: (res: any) => {
        const data = res.data || res;
        this.analyticsData.set(data);
        this.isAnalyticsLoading.set(false);
      },
      error: (err: any) => {
        console.error('Error loading analytics', err);
        this.toast.error('Failed to load PayTabs analytics data.');
        this.isAnalyticsLoading.set(false);
      }
    });
  }

  onFilterChange() {
    this.currentPage.set(1);
    this.loadPayments();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadPayments();
  }

  getPages(): number[] {
    const total = this.totalPages();
    const current = this.currentPage();
    const pages: number[] = [];
    const start = Math.max(1, current - 2);
    const end = Math.min(total, current + 2);
    for (let i = start; i <= end; i++) pages.push(i);
    return pages;
  }

  getStatusClass(status: any): string {
    const s = typeof status === 'string' ? status : (status === 0 ? 'Pending' : status === 1 ? 'Completed' : 'Failed');
    if (s === 'Completed') return 'status-completed';
    if (s === 'Pending') return 'status-pending';
    return 'status-failed';
  }

  getStatusLabel(status: any): string {
    if (typeof status === 'string') return status;
    if (status === 0) return 'Pending';
    if (status === 1) return 'Completed';
    return 'Failed';
  }

  exportCsv(): void {
    const items = this.payments();
    if (!items.length) {
      this.toast.warning('No data to export.');
      return;
    }

    const headers = ['Order ID', 'User', 'Amount', 'Method', 'Status', 'Paid At'];
    const rows = items.map((p: any) => [
      p.orderId || '',
      p.userFullName || '',
      p.amount || 0,
      p.paymentMethod || '',
      this.getStatusLabel(p.paymentStatus),
      p.paidAt || ''
    ]);

    const csv = [headers.join(','), ...rows.map(r => r.join(','))].join('\n');
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `transactions_${new Date().toISOString().slice(0, 10)}.csv`;
    link.click();
    URL.revokeObjectURL(link.href);
    this.toast.success('CSV exported successfully.');
  }
}

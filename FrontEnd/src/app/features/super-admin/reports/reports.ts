import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportsService } from './reports.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reports.html'
})
export class Reports implements OnInit {
  private reportsService = inject(ReportsService);
  private toast = inject(ToastService);

  engineCapacity: number = 0;

  dateStart = signal('');
  dateEnd = signal('');
  reportTypes = ['Financial Summary', 'Operational Metrics', 'User Activity'];

  summary = signal<any>({
    revenue: 0,
    platformFees: 0,
    revenueGrowth: '0% VS LAST MONTH',
    transactionsAvg: 'AVERAGE 0/DAY'
  });
  transactions = signal<any[]>([]);

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.reportsService.getOverviewStats().subscribe({
      next: (response: any) => {
        const actualData = response.data || response;
        this.summary.update(s => ({
          ...s,
          revenue: actualData.grossRevenue || 0,
          platformFees: actualData.platformFees || 0
        }));

        if (actualData.engineCapacity !== undefined) {
          this.engineCapacity = actualData.engineCapacity;
        }
      },
      error: (error: any) => console.error('Error fetching overview stats:', error)
    });

    this.reportsService.getFinancials().subscribe({
      next: (response: any) => {
        const actualData = response.data || response;
        this.transactions.set(actualData.transactions || (Array.isArray(actualData) ? actualData : []));
      },
      error: (error: any) => console.error('Error fetching financials:', error)
    });
  }

  onDateChange(): void {
    this.loadData();
  }

  exportCsv(): void {
    if (!this.transactions() || this.transactions().length === 0) {
      this.toast.warning('No data to export.');
      return;
    }

    const headers = ['Month', 'Revenue', 'Platform Fees', 'Net Revenue'];
    const rows = this.transactions().map((t: any) => [
      t.month || t.label || '',
      t.revenue || t.grossRevenue || 0,
      t.platformFees || t.fees || 0,
      t.netRevenue || (t.revenue || 0) - (t.platformFees || 0)
    ]);

    const csvContent = [headers.join(','), ...rows.map(r => r.join(','))].join('\n');
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `reports_${new Date().toISOString().slice(0, 10)}.csv`;
    link.click();
    URL.revokeObjectURL(link.href);
    this.toast.success('CSV exported successfully.');
  }
}
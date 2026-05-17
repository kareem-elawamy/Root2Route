import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportsService } from './reports.service';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.html'
})
export class Reports implements OnInit {
  private reportsService = inject(ReportsService);
  private cdr = inject(ChangeDetectorRef);

  // 🟢 السطر ده هو اللي كان ناقص ومخلي الأنجولار يزعل
  engineCapacity: number = 0;

  // إعدادات الـ UI الثابتة
  dateFilters = { start: 'Oct 01, 2023', end: 'Oct 31, 2023' };
  reportTypes = ['Financial Summary', 'Operational Metrics', 'User Activity'];
  exportFormats = [
    { label: 'PDF', class: 'bg-white text-emerald-700 shadow-sm font-bold' },
    { label: 'Excel', class: 'text-slate-500 hover:text-emerald-600 transition-colors font-medium' },
    { label: 'CSV', class: 'text-slate-500 hover:text-emerald-600 transition-colors font-medium' }
  ];

  // الداتا اللي هتيجي من الباك إند
  summary: any = {
    revenue: 0,
    platformFees: 0,
    revenueGrowth: '0% VS LAST MONTH',
    transactionsAvg: 'AVERAGE 0/DAY'
  };
  transactions: any[] = [];

  ngOnInit(): void {
    // 1. جلب الإحصائيات العامة
    this.reportsService.getOverviewStats().subscribe({
      next: (response: any) => {
        const actualData = response.data || response;

        // ربط القيم اللي الباك إند بيبعتها فعلاً
        this.summary.revenue = actualData.grossRevenue || 0;
        this.summary.platformFees = actualData.platformFees || 0;

        // لو الباك إند بعت engineCapacity في المستقبل، السطر ده هيشغلها:
        if (actualData.engineCapacity !== undefined) {
          this.engineCapacity = actualData.engineCapacity;
        }

        this.cdr.detectChanges();
      },
      error: (error) => console.error('Error fetching overview stats:', error)
    });

    // 2. جلب المعاملات المالية
    this.reportsService.getFinancials().subscribe({
      next: (response: any) => {
        const actualData = response.data || response;
        this.transactions = actualData.transactions || (Array.isArray(actualData) ? actualData : []);
        this.cdr.detectChanges();
      },
      error: (error) => console.error('Error fetching financials:', error)
    });
  }
}
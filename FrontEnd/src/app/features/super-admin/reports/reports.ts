import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportsService } from './reports.service';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.html'
})
export class Reports implements OnInit {
  // Configuration
  dateFilters = {
    start: 'Oct 01, 2023',
    end: 'Oct 31, 2023'
  };

  reportTypes = [
    'Financial Summary',
    'Operational Metrics',
    'User Activity'
  ];

  exportFormats = [
    { label: 'PDF', class: 'bg-white text-emerald-700 shadow-sm font-bold' },
    { label: 'Excel', class: 'text-slate-500 hover:text-emerald-600 transition-colors font-medium' },
    { label: 'CSV', class: 'text-slate-500 hover:text-emerald-600 transition-colors font-medium' }
  ];

  // Engine Status
  engineCapacity: number = 0;

  // Summary Metrics
  summary: any = {
    revenue: '$0.00',
    revenueGrowth: '0% VS LAST MONTH',
    transactions: '0',
    transactionsAvg: 'AVERAGE 0/DAY'
  };

  // Transactions Data
  transactions: any[] = [];

  constructor(private reportsService: ReportsService) {}

  ngOnInit(): void {
    this.reportsService.getOverviewStats().subscribe({
      next: (data: any) => {
        if (data.engineCapacity !== undefined) {
          this.engineCapacity = data.engineCapacity;
        }
        if (data.summary) {
          this.summary = data.summary;
        }
      },
      error: (error) => console.error('Error fetching overview stats:', error)
    });

    this.reportsService.getFinancials().subscribe({
      next: (data: any) => {
        // Handle returning an array directly or an object with a transactions property
        this.transactions = data.transactions || Array.isArray(data) ? data : [];
      },
      error: (error) => console.error('Error fetching financials:', error)
    });
  }
}

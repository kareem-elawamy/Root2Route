import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaymentService } from '../../../core/services/payment.service';
import { ToastService } from '../../../core/services/toast.service';
import { SkeletonComponent } from '../../../shared/components/skeleton/skeleton.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [CommonModule, FormsModule, SkeletonComponent, PaginationComponent],
  templateUrl: './transactions.html',
  styleUrl: './transactions.css'
})
export class Transactions implements OnInit {
  private paymentService = inject(PaymentService);
  private toast = inject(ToastService);

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

  ngOnInit() {
    this.loadPayments();
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
        const items = data.items || data.payments || (Array.isArray(data) ? data : []);
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

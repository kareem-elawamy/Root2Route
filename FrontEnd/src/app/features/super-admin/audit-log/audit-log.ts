import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuditLogService } from '../../../core/services/audit-log.service';
import { ToastService } from '../../../core/services/toast.service';
import { SkeletonComponent } from '../../../shared/components/skeleton/skeleton.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-audit-log',
  standalone: true,
  imports: [CommonModule, FormsModule, SkeletonComponent, PaginationComponent],
  templateUrl: './audit-log.html',
  styleUrl: './audit-log.css'
})
export class AuditLog implements OnInit {
  private auditService = inject(AuditLogService);
  private toast = inject(ToastService);

  logs = signal<any[]>([]);
  isLoading = signal(true);
  filterEntity = signal('');
  filterAction = signal('');
  dateFrom = signal('');
  dateTo = signal('');
  currentPage = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);
  pageSize = 25;

  entityTypes = ['Organization', 'Product', 'Order', 'Auction', 'Payment', 'User'];
  actionTypes = ['Create', 'Update', 'Delete'];

  expandedLogId = signal<string | null>(null);

  ngOnInit() {
    this.loadLogs();
  }

  loadLogs() {
    this.isLoading.set(true);
    this.auditService.getAuditLogs(
      this.currentPage(),
      this.pageSize,
      this.filterEntity() || undefined,
      this.filterAction() || undefined,
      this.dateFrom() || undefined,
      this.dateTo() || undefined
    ).subscribe({
      next: (response: any) => {
        const data = response.data || response;
        const items = data.items || data.logs || (Array.isArray(data) ? data : []);
        this.logs.set(items);
        this.totalCount.set(data.totalCount || items.length);
        this.totalPages.set(Math.ceil(this.totalCount() / this.pageSize) || 1);
        this.isLoading.set(false);
      },
      error: (err: any) => {
        console.error('Error loading audit logs', err);
        this.isLoading.set(false);
      }
    });
  }

  onFilterChange() {
    this.currentPage.set(1);
    this.loadLogs();
  }

  toggleExpand(logId: string) {
    this.expandedLogId.set(this.expandedLogId() === logId ? null : logId);
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadLogs();
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

  getActionClass(action: string): string {
    if (action === 'Create') return 'action-create';
    if (action === 'Update') return 'action-update';
    if (action === 'Delete') return 'action-delete';
    return 'action-other';
  }

  getActionIcon(action: string): string {
    if (action === 'Create') return 'add_circle';
    if (action === 'Update') return 'edit';
    if (action === 'Delete') return 'delete';
    return 'info';
  }

  exportCsv(): void {
    const items = this.logs();
    if (!items.length) {
      this.toast.warning('No data to export.');
      return;
    }

    const headers = ['Timestamp', 'User', 'Action', 'Entity', 'Old Values', 'New Values'];
    const rows = items.map((l: any) => [
      l.createdAt || '',
      `"${l.userFullName || ''}"`,
      l.action || '',
      l.entityName || '',
      `"${(l.oldValues || '').replace(/"/g, '""')}"`,
      `"${(l.newValues || '').replace(/"/g, '""')}"`
    ]);

    const csv = [headers.join(','), ...rows.map(r => r.join(','))].join('\n');
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `audit_log_${new Date().toISOString().slice(0, 10)}.csv`;
    link.click();
    URL.revokeObjectURL(link.href);
    this.toast.success('CSV exported successfully.');
  }
}

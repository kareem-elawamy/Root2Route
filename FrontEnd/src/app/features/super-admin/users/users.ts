import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminUserService } from '../../../core/services/admin-user.service';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmDialogService } from '../../../core/services/confirm-dialog.service';
import { SkeletonComponent } from '../../../shared/components/skeleton/skeleton.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule, SkeletonComponent, PaginationComponent],
  templateUrl: './users.html',
  styleUrl: './users.css'
})
export class Users implements OnInit {
  private userService = inject(AdminUserService);
  private toast = inject(ToastService);
  private confirmDialog = inject(ConfirmDialogService);

  users = signal<any[]>([]);
  isLoading = signal(true);
  searchTerm = signal('');
  filterStatus = signal<string>('all');
  currentPage = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);
  pageSize = 15;

  // Detail drawer
  selectedUser = signal<any | null>(null);
  isDrawerOpen = signal(false);

  // Stats
  stats = signal({ total: 0, active: 0, blocked: 0 });

  // Chart Data (Status Distribution)
  chartData = computed(() => {
    const s = this.stats();
    const total = s.total || 1; // Prevent div by 0
    return {
      activePercent: Math.round((s.active / total) * 100),
      blockedPercent: Math.round((s.blocked / total) * 100),
    };
  });

  private searchTimeout: any;

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.isLoading.set(true);
    const blocked = this.filterStatus() === 'blocked' ? true :
                      this.filterStatus() === 'active' ? false : undefined;

    this.userService.getAllUsers(
      this.currentPage(),
      this.pageSize,
      this.searchTerm() || undefined,
      blocked
    ).subscribe({
      next: (response: any) => {
        const data = response.data || response;
        const items = data.data || data.items || data.users || (Array.isArray(data) ? data : []);
        this.users.set(items);
        this.totalCount.set(data.totalCount || items.length);
        this.totalPages.set(Math.ceil(this.totalCount() / this.pageSize) || 1);

        this.stats.set({
          total: data.totalCount || items.length,
          active: items.filter((u: any) => !u.isDeleted && !u.isBlocked).length,
          blocked: items.filter((u: any) => u.isDeleted || u.isBlocked).length
        });

        this.isLoading.set(false);
      },
      error: (err: any) => {
        console.error('Error loading users', err);
        this.toast.error('Failed to load users.');
        this.isLoading.set(false);
      }
    });
  }

  onSearchChange(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      this.currentPage.set(1);
      this.loadUsers();
    }, 400);
  }

  onFilterChange(status: string) {
    this.filterStatus.set(status);
    this.currentPage.set(1);
    this.loadUsers();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadUsers();
  }



  openDetail(user: any) {
    this.selectedUser.set(user);
    this.isDrawerOpen.set(true);
  }

  closeDetail() {
    this.isDrawerOpen.set(false);
    this.selectedUser.set(null);
  }

  async toggleBlock(user: any) {
    const isBlocked = user.isDeleted || user.isBlocked;
    const action = isBlocked ? 'reactivate' : 'block';

    const confirmed = await this.confirmDialog.open({
      title: `${isBlocked ? 'Reactivate' : 'Block'} User`,
      message: `Are you sure you want to ${action} ${user.fullName || user.email}?`,
      confirmLabel: isBlocked ? 'Reactivate' : 'Block',
      isDestructive: !isBlocked
    });
    if (!confirmed) return;

    this.userService.blockUser(user.id, !isBlocked).subscribe({
      next: () => {
        this.toast.success(`User ${action}d successfully.`);
        this.loadUsers();
        this.closeDetail();
      },
      error: (err: any) => {
        console.error(err);
        this.toast.error(`Failed to ${action} user.`);
      }
    });
  }

  getUserInitial(user: any): string {
    return (user.fullName || user.email || '?').charAt(0).toUpperCase();
  }

  exportCsv() {
    const items = this.users();
    if (!items.length) {
      this.toast.error('No users to export.');
      return;
    }

    const headers = ['ID', 'Full Name', 'Email', 'Phone', 'Organizations', 'Joined', 'Status'];
    const rows = items.map((u: any) => [
      u.id || '',
      `"${(u.fullName || '').replace(/"/g, '""')}"`,
      u.email || '',
      u.phoneNumber || '',
      u.organizationCount || 0,
      u.createdAt || '',
      (u.isDeleted || u.isBlocked) ? 'Blocked' : 'Active'
    ]);

    const csv = [headers.join(','), ...rows.map(r => r.join(','))].join('\n');
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `users_${new Date().toISOString().slice(0, 10)}.csv`;
    link.click();
    URL.revokeObjectURL(link.href);
    this.toast.success('Users exported successfully.');
  }
}

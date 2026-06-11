import { Component, OnInit, inject, signal } from '@angular/core';
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
  stats = signal({ total: 0, active: 0, suspended: 0 });

  private searchTimeout: any;

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.isLoading.set(true);
    const suspended = this.filterStatus() === 'suspended' ? true :
                      this.filterStatus() === 'active' ? false : undefined;

    this.userService.getAllUsers(
      this.currentPage(),
      this.pageSize,
      this.searchTerm() || undefined,
      suspended
    ).subscribe({
      next: (response: any) => {
        const data = response.data || response;
        const items = data.items || data.users || (Array.isArray(data) ? data : []);
        this.users.set(items);
        this.totalCount.set(data.totalCount || items.length);
        this.totalPages.set(Math.ceil(this.totalCount() / this.pageSize) || 1);

        this.stats.set({
          total: data.totalCount || items.length,
          active: items.filter((u: any) => !u.isDeleted && !u.isSuspended).length,
          suspended: items.filter((u: any) => u.isDeleted || u.isSuspended).length
        });

        this.isLoading.set(false);
      },
      error: (err: any) => {
        console.error('Error loading users', err);
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

  toggleSuspend(user: any) {
    const isSuspended = user.isDeleted || user.isSuspended;
    const action = isSuspended ? 'reactivate' : 'suspend';

    this.confirmDialog.open({
      title: `${isSuspended ? 'Reactivate' : 'Suspend'} User`,
      message: `Are you sure you want to ${action} ${user.fullName || user.email}?`,
      confirmLabel: isSuspended ? 'Reactivate' : 'Suspend',
      isDestructive: !isSuspended
    }).subscribe((confirmed: any) => {
      if (!confirmed) return;

      this.userService.suspendUser(user.id, !isSuspended).subscribe({
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
    });
  }

  getUserInitial(user: any): string {
    return (user.fullName || user.email || '?').charAt(0).toUpperCase();
  }
}

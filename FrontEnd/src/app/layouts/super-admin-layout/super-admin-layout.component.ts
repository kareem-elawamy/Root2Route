import { Component, inject, signal, HostListener, ElementRef } from '@angular/core';
import { Router, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-super-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css'
})
export class SuperAdminLayoutComponent {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly elRef = inject(ElementRef);
  private readonly notifService = inject(NotificationService);

  readonly currentUser = this.authService.currentUser;

  // Sidebar
  sidebarCollapsed = signal(false);

  // Notifications
  unreadCount = signal(0);
  notifications = signal<any[]>([]);
  showNotifications = signal(false);

  ngOnInit(): void {
    this.loadNotifications();
  }

  loadNotifications(): void {
    this.notifService.getUnreadCount().subscribe({
      next: (res: any) => this.unreadCount.set(res?.data || res || 0),
      error: () => {}
    });
    this.notifService.getNotifications().subscribe({
      next: (res: any) => this.notifications.set(res?.data || res || []),
      error: () => {}
    });
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!this.elRef.nativeElement.contains(target)) {
      this.showNotifications.set(false);
    }
  }

  toggleSidebar(): void {
    this.sidebarCollapsed.update(v => !v);
  }

  toggleNotifications(): void {
    this.showNotifications.update(v => !v);
    if (this.showNotifications()) {
      this.notifService.markAllAsRead().subscribe(() => {
        this.unreadCount.set(0);
      });
    }
  }

  userInitial(): string {
    return this.currentUser()?.name?.charAt(0)?.toUpperCase() ?? 'A';
  }

  logout(): void {
    this.authService.clearSession();
    this.router.navigate(['/auth/login']);
  }
}
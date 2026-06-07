import { Component, OnInit, inject, signal, HostListener, ElementRef } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrgContextService } from '../../core/services/org-context.service';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { OrganizationService } from '../../features/super-admin/organizations/organization.service';

@Component({
  selector: 'app-org-layout-component',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule, FormsModule],
  templateUrl: './org-layout-component.html',
  styleUrl: './org-layout-component.css',
})
export class OrgLayoutComponent implements OnInit {
  private readonly orgCtx = inject(OrgContextService);
  private readonly elRef = inject(ElementRef);
  private readonly router = inject(Router);
  readonly auth = inject(AuthService);

  readonly organizations = this.orgCtx.organizations;
  readonly activeOrg = this.orgCtx.activeOrg;
  readonly dropdownOpen = signal(false);
  readonly isLoadingOrgs = signal(false);

  readonly currentUser = this.auth.currentUser;

  // Notifications
  private readonly notifService = inject(NotificationService);
  unreadCount = signal(0);
  notifications = signal<any[]>([]);
  showNotifications = signal(false);

  // Create Org Modal
  private readonly orgApi = inject(OrganizationService);
  showCreateModal = signal(false);
  isCreatingOrg = signal(false);

  ngOnInit(): void {
    this.loadOrgs();
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
      this.dropdownOpen.set(false);
      this.showNotifications.set(false);
    }
  }

  loadOrgs(): void {
    this.isLoadingOrgs.set(true);
    this.orgCtx.myOrganization().subscribe({
      next: () => this.isLoadingOrgs.set(false),
      error: () => this.isLoadingOrgs.set(false),
    });
  }

  toggleDropdown(): void {
    this.dropdownOpen.update((v) => !v);
  }

  closeDropdown(): void {
    this.dropdownOpen.set(false);
  }

  logout(): void {
    this.auth.clearSession();
    this.router.navigate(['/auth/login']);
  }

  switchOrg(orgId: string): void {
    this.orgCtx.switchOrganization(orgId);
    this.dropdownOpen.set(false);
  }

  /** First letter of org name for avatar fallback */
  orgInitial(name: string): string {
    return name?.charAt(0)?.toUpperCase() ?? '?';
  }

  /** First letter of user name */
  userInitial(): string {
    return this.currentUser()?.name?.charAt(0)?.toUpperCase() ?? 'U';
  }

  actionPlaceholder(actionName: string): void {
    alert(`Feature '${actionName}' is coming soon!`);
  }

  toggleNotifications(): void {
    this.showNotifications.update(v => !v);
    if (this.showNotifications()) {
      this.dropdownOpen.set(false);
      this.notifService.markAllAsRead().subscribe(() => {
        this.unreadCount.set(0);
      });
    }
  }

  openCreateModal(): void {
    this.showCreateModal.set(true);
    this.dropdownOpen.set(false);
  }

  closeCreateModal(): void {
    this.showCreateModal.set(false);
  }

  submitOrg(event: Event): void {
    event.preventDefault();
    const form = event.target as HTMLFormElement;
    
    const name = (form.elements.namedItem('orgName') as HTMLInputElement).value;
    if (!name) return;

    const formData = new FormData();
    formData.append('Name', name);
    formData.append('Description', (form.elements.namedItem('orgDesc') as HTMLInputElement).value);
    formData.append('Type', (form.elements.namedItem('orgType') as HTMLSelectElement).value);

    this.isCreatingOrg.set(true);
    this.orgApi.createOrganization(formData).subscribe({
      next: () => {
        this.isCreatingOrg.set(false);
        this.closeCreateModal();
        this.loadOrgs();
        alert('Organization created successfully!');
      },
      error: (err) => {
        this.isCreatingOrg.set(false);
        alert('Error creating organization.');
        console.error(err);
      }
    });
  }
}

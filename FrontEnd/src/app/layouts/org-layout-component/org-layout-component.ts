import { Component, OnInit, inject, signal, HostListener, ElementRef } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { OrgContextService } from '../../core/services/org-context.service';
import { AuthService } from '../../core/services/auth.service';
import { MyOrganization } from '../../core/model/Organization/myOrganization';

@Component({
  selector: 'app-org-layout-component',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
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

  ngOnInit(): void {
    this.loadOrgs();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elRef.nativeElement.contains(event.target)) {
      this.dropdownOpen.set(false);
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
}

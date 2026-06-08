import { Component, OnInit, inject, ChangeDetectorRef, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MembersService } from '../../members.service';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-roles',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './roles.html',
  styleUrl: './roles.css'
})
export class RolesComponent implements OnInit {
  private membersService = inject(MembersService);
  private orgCtx = inject(OrgContextService);
  private cdr = inject(ChangeDetectorRef);
  public authService = inject(AuthService);

  readonly activeOrg = this.orgCtx.activeOrg;
  
  roles: any[] = [];
  systemPermissions: string[] = [];
  
  isCreateModalOpen = false;
  isCreating = false;
  isHelpOpen = false;
  
  newRole = {
    name: '',
    description: '',
    permissions: new Set<string>()
  };

  constructor() {
    effect(() => {
      const org = this.activeOrg();
      if (org && org.id) {
        this.loadRoles(org.id);
      }
    });
  }

  ngOnInit() {
    this.loadSystemPermissions();
  }

  loadRoles(orgId: string) {
    this.membersService.getOrganizationRoles(orgId).subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.roles = Array.isArray(data) ? data : [];
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error fetching roles', err)
    });
  }

  loadSystemPermissions() {
    this.membersService.getSystemPermissions().subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.systemPermissions = Array.isArray(data) ? data : [];
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error fetching system permissions', err)
    });
  }

  openCreateModal() {
    this.newRole = { name: '', description: '', permissions: new Set<string>() };
    this.isCreateModalOpen = true;
  }

  closeCreateModal() {
    this.isCreateModalOpen = false;
  }

  toggleRolesHelp() {
    this.isHelpOpen = !this.isHelpOpen;
  }

  closeRolesHelp() {
    this.isHelpOpen = false;
  }

  togglePermission(permission: any) {
    // some APIs return permission objects, some return strings
    const permKey = permission?.permissionsClaim || permission?.name || permission;
    if (this.newRole.permissions.has(permKey)) {
      this.newRole.permissions.delete(permKey);
    } else {
      this.newRole.permissions.add(permKey);
    }
  }

  hasPermission(permission: any): boolean {
    const permKey = permission?.permissionsClaim || permission?.name || permission;
    return this.newRole.permissions.has(permKey);
  }

  createRole() {
    const org = this.activeOrg();
    if (!org || !org.id) return;
    
    if (!this.newRole.name) {
      alert('Role Name is required');
      return;
    }

    this.isCreating = true;
    const command = {
      name: this.newRole.name,
      description: this.newRole.description,
      permissions: Array.from(this.newRole.permissions),
      isSystemDefault: false
    };

    this.membersService.createOrganizationRole(command).subscribe({
      next: () => {
        this.isCreating = false;
        this.closeCreateModal();
        this.loadRoles(org.id);
      },
      error: (err) => {
        console.error('Error creating role', err);
        this.isCreating = false;
        
        // Try to extract exact backend error message
        let errorMsg = 'Failed to create role';
        if (err.status === 403) {
          errorMsg = "You don't have permission to create roles (Forbidden).";
        } else if (err.error?.meta?.message) {
          errorMsg = err.error.meta.message;
        } else if (typeof err.error === 'string') {
          errorMsg = err.error;
        } else if (err.message) {
          errorMsg = err.message;
        }
        
        alert(errorMsg);
      }
    });
  }
}

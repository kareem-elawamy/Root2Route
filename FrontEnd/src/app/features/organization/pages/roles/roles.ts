import { Component, OnInit, inject, effect, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MembersService } from '../../members.service';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { ConfirmDialogService } from '../../../../core/services/confirm-dialog.service';
import { SkeletonComponent } from '../../../../shared/components/skeleton/skeleton.component';

@Component({
  selector: 'app-roles',
  standalone: true,
  imports: [CommonModule, FormsModule, SkeletonComponent],
  templateUrl: './roles.html',
  styleUrl: './roles.css'
})
export class RolesComponent implements OnInit {
  private membersService = inject(MembersService);
  private orgCtx = inject(OrgContextService);
  private toast = inject(ToastService);
  private confirmDialog = inject(ConfirmDialogService);
  public authService = inject(AuthService);

  readonly activeOrg = this.orgCtx.activeOrg;
  
  roles = signal<any[]>([]);
  systemPermissions = signal<string[]>([]);
  
  isCreateModalOpen = signal(false);
  isCreating = signal(false);
  isHelpOpen = signal(false);
  isLoading = signal(true);
  editingRoleId = signal<string | null>(null);
  
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
    this.isLoading.set(true);
    this.membersService.getOrganizationRoles(orgId).subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.roles.set(Array.isArray(data) ? data : []);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error fetching roles', err);
        this.isLoading.set(false);
      }
    });
  }

  loadSystemPermissions() {
    this.membersService.getSystemPermissions().subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.systemPermissions.set(Array.isArray(data) ? data : []);
      },
      error: (err) => console.error('Error fetching system permissions', err)
    });
  }

  openCreateModal() {
    this.newRole = { name: '', description: '', permissions: new Set<string>() };
    this.editingRoleId.set(null);
    this.isCreateModalOpen.set(true);
  }

  closeCreateModal() {
    this.isCreateModalOpen.set(false);
    this.editingRoleId.set(null);
  }

  toggleRolesHelp() {
    this.isHelpOpen.update(v => !v);
  }

  closeRolesHelp() {
    this.isHelpOpen.set(false);
  }

  togglePermission(permission: any) {
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

  saveRole() {
    const org = this.activeOrg();
    if (!org || !org.id) return;
    
    if (!this.newRole.name) {
      this.toast.error('Role Name is required');
      return;
    }

    this.isCreating.set(true);
    const command = {
      name: this.newRole.name,
      description: this.newRole.description,
      permissions: Array.from(this.newRole.permissions),
      isSystemDefault: false
    };

    const roleId = this.editingRoleId();

    if (roleId) {
      // UPDATE existing role
      this.membersService.updateRole(roleId, command).subscribe({
        next: () => {
          this.isCreating.set(false);
          this.toast.success('Role updated successfully');
          this.closeCreateModal();
          this.loadRoles(org.id);
        },
        error: (err) => {
          console.error('Error updating role', err);
          this.isCreating.set(false);
          this.toast.error(this.extractError(err, 'Failed to update role'));
        }
      });
    } else {
      // CREATE new role
      this.membersService.createOrganizationRole(command).subscribe({
        next: () => {
          this.isCreating.set(false);
          this.toast.success('Role created successfully');
          this.closeCreateModal();
          this.loadRoles(org.id);
        },
        error: (err) => {
          console.error('Error creating role', err);
          this.isCreating.set(false);
          this.toast.error(this.extractError(err, 'Failed to create role'));
        }
      });
    }
  }

  editRole(role: any) {
    this.newRole = {
      name: role.name || '',
      description: role.description || '',
      permissions: new Set<string>(role.permissions || role.organizationRolePermissions?.map((p: any) => p.permissionsClaim) || [])
    };
    this.editingRoleId.set(role.id);
    this.isCreateModalOpen.set(true);
  }

  deleteRole(roleId: string) {
    this.confirmDialog.open({
      title: 'Delete Role',
      message: 'Are you sure you want to delete this role? Members with this role will be unassigned.',
      confirmLabel: 'Delete',
      isDestructive: true
    }).subscribe((confirmed: any) => {
      if (!confirmed) return;
      this.membersService.deleteRole(roleId).subscribe({
        next: () => {
          this.toast.success('Role deleted successfully.');
          const org = this.activeOrg();
          if (org) this.loadRoles(org.id);
        },
        error: (err: any) => {
          console.error('Error deleting role', err);
          this.toast.error('Failed to delete role.');
        }
      });
    });
  }

  private extractError(err: any, fallback: string): string {
    if (err.status === 403) return "You don't have permission for this action (Forbidden).";
    if (err.error?.meta?.message) return err.error.meta.message;
    if (typeof err.error === 'string') return err.error;
    if (err.message) return err.message;
    return fallback;
  }
}

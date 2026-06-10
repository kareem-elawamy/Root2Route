import { Component, OnInit, inject, effect, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MembersService } from '../../members.service';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { AuthService } from '../../../../core/services/auth.service';
import { FormsModule } from '@angular/forms';
import { ToastService } from '../../../../core/services/toast.service';
import { ConfirmDialogService } from '../../../../core/services/confirm-dialog.service';

import { SkeletonComponent } from '../../../../shared/components/skeleton/skeleton.component';

@Component({
  selector: 'app-members',
  standalone: true,
  imports: [CommonModule, FormsModule, SkeletonComponent],
  templateUrl: './members.html',
  styleUrl: './members.css'
})
export class MembersComponent implements OnInit {
  private membersService = inject(MembersService);
  private orgCtx = inject(OrgContextService);

  public authService = inject(AuthService);
  private toast = inject(ToastService);
  private confirmDialog = inject(ConfirmDialogService);

  readonly activeOrg = this.orgCtx.activeOrg;
  members = signal<any[]>([]);
  invitations = signal<any[]>([]);
  roles = signal<any[]>([]);
  
  activeTab = signal<'members' | 'invitations'>('members');
  isHelpOpen = signal(false);
  isLoadingMembers = signal(true);
  isLoadingInvitations = signal(true);
  
  newInvite = {
    email: '',
    roleId: '',
    expiryDate: this.getDefaultExpiryDate()
  };

  getDefaultExpiryDate(): string {
    const d = new Date();
    d.setDate(d.getDate() + 7);
    return d.toISOString().split('T')[0];
  }

  constructor() {
    effect(() => {
      const org = this.activeOrg();
      if (org && org.id) {
        this.loadData(org.id);
      }
    });
  }

  ngOnInit() {
  }

  loadData(orgId: string) {
    this.loadMembers(orgId);
    this.loadInvitations(orgId);
    this.loadRoles(orgId);
  }

  loadRoles(orgId: string) {
    this.membersService.getOrganizationRoles(orgId).subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.roles.set(Array.isArray(data) ? data : []);
      },
      error: (error: any) => {
        console.error('Error fetching roles', error);
      }
    });
  }

  loadMembers(orgId: string) {
    this.isLoadingMembers.set(true);
    this.membersService.getMembers(orgId).subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.members.set(Array.isArray(data) ? data : []);
        this.isLoadingMembers.set(false);
      },
      error: (error: any) => {
        console.error('Error fetching members', error);
        this.isLoadingMembers.set(false);
      }
    });
  }

  loadInvitations(orgId: string) {
    this.isLoadingInvitations.set(true);
    this.membersService.getInvitations(orgId).subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.invitations.set(Array.isArray(data) ? data : []);
        this.isLoadingInvitations.set(false);
      },
      error: (error: any) => {
        console.error('Error fetching invitations', error);
        this.isLoadingInvitations.set(false);
      }
    });
  }

  sendInvite() {
    const org = this.activeOrg();
    if (!org || !org.id) return;
    if (!this.newInvite.roleId) {
      this.toast.warning('Please select a role.');
      return;
    }

    const command = {
      email: this.newInvite.email,
      organizationId: org.id,
      roleId: this.newInvite.roleId,
      expiryDate: new Date(this.newInvite.expiryDate).toISOString()
    };

    this.membersService.sendInvitation(command).subscribe({
      next: () => {
        this.toast.success('Invitation sent successfully!');
        this.newInvite.email = '';
        this.newInvite.roleId = '';
        this.newInvite.expiryDate = this.getDefaultExpiryDate();
        this.loadInvitations(org.id);
      },
      error: (error: any) => {
        console.error('Error sending invitation', error);
        this.toast.error('Failed to send invitation.');
      }
    });
  }

  removeMember(memberId: string) {
    this.confirmDialog.open({
      title: 'Remove Member',
      message: 'Are you sure you want to remove this member?',
      confirmLabel: 'Remove',
      isDestructive: true
    }).subscribe(confirmed => {
      if (!confirmed) return;

      this.membersService.removeMember(memberId).subscribe({
        next: () => {
          this.toast.success('Member removed successfully!');
          const org = this.activeOrg();
          if (org) this.loadMembers(org.id);
        },
        error: (error: any) => {
          console.error('Error removing member', error);
          this.toast.error('Failed to remove member.');
        }
      });
    });
  }

  revokeInvitation(invitationId: string) {
    this.confirmDialog.open({
      title: 'Revoke Invitation',
      message: 'Are you sure you want to revoke this invitation?',
      confirmLabel: 'Revoke',
      isDestructive: true
    }).subscribe(confirmed => {
      if (!confirmed) return;

      this.membersService.revokeInvitation(invitationId).subscribe({
        next: () => {
          this.toast.success('Invitation revoked successfully!');
          const org = this.activeOrg();
          if (org) this.loadInvitations(org.id);
        },
        error: (error: any) => {
          console.error('Error revoking invitation', error);
          this.toast.error('Failed to revoke invitation.');
        }
      });
    });
  }

  setTab(tab: 'members' | 'invitations') {
    this.activeTab.set(tab);
  }

  toggleMembersHelp() {
    this.isHelpOpen.update(v => !v);
  }

  closeMembersHelp() {
    this.isHelpOpen.set(false);
  }

  getStatusText(status: number | string): string {
    if (typeof status === 'string') return status;
    switch(status) {
      case 0: return 'Pending';
      case 1: return 'Accepted';
      case 2: return 'Expired';
      case 3: return 'Revoked';
      default: return 'Unknown';
    }
  }

  changeOwner(memberId: string) {
    const org = this.activeOrg();
    if (!org) return;

    this.confirmDialog.open({
      title: 'Transfer Ownership',
      message: 'Are you sure you want to transfer ownership to this member? You will lose Owner privileges.',
      confirmLabel: 'Transfer',
      isDestructive: true
    }).subscribe(confirmed => {
      if (!confirmed) return;

      this.membersService.changeOrganizationOwner(org.id, memberId).subscribe({
        next: () => {
          this.toast.success('Ownership transferred successfully! Please login again to refresh permissions.');
          this.authService.clearSession();
          window.location.href = '/login';
        },
        error: (err: any) => {
          console.error(err);
          this.toast.error('Error transferring ownership.');
        }
      });
    });
  }

  get totalMembers(): number {
    return this.members().length;
  }

  get roleDistribution() {
    const dist = new Map<string, number>();
    const orgOwnerId = this.activeOrg()?.ownerId;

    for (const m of this.members()) {
      let role = 'MEMBER';
      if (m.roles && m.roles.length > 0) {
        // Ensure consistent casing for grouping
        role = m.roles.join(', ').toUpperCase();
      } else if (m.userId === orgOwnerId) {
        role = 'OWNER';
      }
      
      dist.set(role, (dist.get(role) || 0) + 1);
    }
    const colors = ['#007aff', '#34c759', '#ff9500', '#ff3b30', '#af52de']; // Apple-like colors
    let i = 0;
    return Array.from(dist.entries()).map(([role, count]) => ({
      role,
      count,
      color: colors[i++ % colors.length]
    }));
  }

  getDonutGradient(): string {
    const total = this.totalMembers || 1;
    let gradientParts = [];
    let currentPercentage = 0;

    for (const item of this.roleDistribution) {
      const percentage = (item.count / total) * 100;
      gradientParts.push(`${item.color} ${currentPercentage}% ${currentPercentage + percentage}%`);
      currentPercentage += percentage;
    }

    if (gradientParts.length === 0) {
      return 'conic-gradient(#e2e8f0 0% 100%)';
    }

    return `conic-gradient(${gradientParts.join(', ')})`;
  }

  assignRoleToMember(memberId: string, roleId: string) {
    if (!roleId) return;
    this.membersService.assignRoleToMember(memberId, roleId).subscribe({
      next: () => {
        this.toast.success('Role assigned successfully.');
        const org = this.activeOrg();
        if (org) this.loadMembers(org.id);
      },
      error: (err: any) => {
        console.error('Error assigning role', err);
        this.toast.error('Failed to assign role.');
      }
    });
  }
}

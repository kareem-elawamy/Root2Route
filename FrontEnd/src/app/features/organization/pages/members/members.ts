import { Component, OnInit, inject, ChangeDetectorRef, effect, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MembersService } from '../../members.service';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { AuthService } from '../../../../core/services/auth.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-members',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './members.html',
  styleUrl: './members.css'
})
export class MembersComponent implements OnInit {
  private membersService = inject(MembersService);
  private orgCtx = inject(OrgContextService);
  private cdr = inject(ChangeDetectorRef);
  public authService = inject(AuthService);

  readonly activeOrg = this.orgCtx.activeOrg;
  members: any[] = [];
  invitations: any[] = [];
  roles: any[] = [];
  
  activeTab = signal<'members' | 'invitations'>('members');
  isHelpOpen = signal(false);
  
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
        this.roles = Array.isArray(data) ? data : [];
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error fetching roles', error);
      }
    });
  }

  loadMembers(orgId: string) {
    this.membersService.getMembers(orgId).subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.members = Array.isArray(data) ? data : [];
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error fetching members', error);
      }
    });
  }

  loadInvitations(orgId: string) {
    this.membersService.getInvitations(orgId).subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.invitations = Array.isArray(data) ? data : [];
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error fetching invitations', error);
      }
    });
  }

  sendInvite() {
    const org = this.activeOrg();
    if (!org || !org.id) return;
    if (!this.newInvite.roleId) {
      alert('Please select a role.');
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
        alert('Invitation sent successfully!');
        this.newInvite.email = '';
        this.newInvite.roleId = '';
        this.newInvite.expiryDate = this.getDefaultExpiryDate();
        this.loadInvitations(org.id);
      },
      error: (error: any) => {
        console.error('Error sending invitation', error);
        alert('Failed to send invitation.');
      }
    });
  }

  removeMember(memberId: string) {
    if (!confirm('Are you sure you want to remove this member?')) return;

    this.membersService.removeMember(memberId).subscribe({
      next: () => {
        alert('Member removed successfully!');
        const org = this.activeOrg();
        if (org) this.loadMembers(org.id);
      },
      error: (error: any) => {
        console.error('Error removing member', error);
        alert('Failed to remove member.');
      }
    });
  }

  revokeInvitation(invitationId: string) {
    if (!confirm('Are you sure you want to revoke this invitation?')) return;

    this.membersService.revokeInvitation(invitationId).subscribe({
      next: () => {
        alert('Invitation revoked successfully!');
        const org = this.activeOrg();
        if (org) this.loadInvitations(org.id);
      },
      error: (error: any) => {
        console.error('Error revoking invitation', error);
        alert('Failed to revoke invitation.');
      }
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
    if (!confirm('Are you sure you want to transfer ownership to this member? You will lose Owner privileges.')) return;
    const org = this.activeOrg();
    if (!org) return;

    // Call the change owner API
    const token = localStorage.getItem('access_token');
    fetch(`https://root2route.runasp.net/api/v1/organizations/${org.id}/change-owner`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({ newOwnerId: memberId }) // Assuming the body expects newOwnerId
    })
    .then(async res => {
      if (!res.ok) {
        const txt = await res.text();
        throw new Error(txt || 'Failed to change owner');
      }
      return res.json();
    })
    .then(() => {
      alert('Ownership transferred successfully! Please login again to refresh permissions.');
      this.authService.clearSession();
      window.location.href = '/login';
    })
    .catch(err => {
      console.error(err);
      alert('Error transferring ownership: ' + err.message);
    });
  }

  get totalMembers(): number {
    return this.members.length;
  }

  get roleDistribution() {
    const dist = new Map<string, number>();
    const orgOwnerId = this.activeOrg()?.ownerId;

    for (const m of this.members) {
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
}

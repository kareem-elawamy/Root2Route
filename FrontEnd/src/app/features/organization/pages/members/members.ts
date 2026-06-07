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
}

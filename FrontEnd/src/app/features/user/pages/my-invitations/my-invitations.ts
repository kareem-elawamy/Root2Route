import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MembersService } from '../../../organization/members.service';
import { Router } from '@angular/router';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-my-invitations',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-invitations.html'
})
export class MyInvitationsComponent implements OnInit {
  private membersService = inject(MembersService);
  private orgCtx = inject(OrgContextService);

  private router = inject(Router);
  private toast = inject(ToastService);

  invitations = signal<any[]>([]);
  isLoading = signal(true);

  ngOnInit() {
    this.loadInvitations();
  }

  loadInvitations() {
    this.isLoading.set(true);
    this.membersService.getMyInvitations().subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.invitations.set(Array.isArray(data) ? data : []);
        this.isLoading.set(false);
      },
      error: (err: any) => {
        console.error('Error fetching my invitations', err);
        this.isLoading.set(false);
      }
    });
  }

  acceptInvitation(invitationId: string, token: string) {
    this.membersService.acceptInvitation(invitationId, token).subscribe({
      next: () => {
        this.toast.success('Invitation accepted successfully!');
        // Reload organizations so the user can now access their new org
        this.orgCtx.myOrganization().subscribe(() => {
          this.router.navigate(['/org']);
        });
      },
      error: (err: any) => {
        console.error('Error accepting invitation', err);
        this.toast.error('Failed to accept invitation.');
      }
    });
  }

  logout() {
    localStorage.clear();
    this.router.navigate(['/auth/login']);
  }
}

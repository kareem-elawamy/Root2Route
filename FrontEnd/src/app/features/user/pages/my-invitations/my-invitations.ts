import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MembersService } from '../../../organization/members.service';
import { Router } from '@angular/router';
import { OrgContextService } from '../../../../core/services/org-context.service';

@Component({
  selector: 'app-my-invitations',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-invitations.html'
})
export class MyInvitationsComponent implements OnInit {
  private membersService = inject(MembersService);
  private orgCtx = inject(OrgContextService);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);

  invitations: any[] = [];
  isLoading = true;

  ngOnInit() {
    this.loadInvitations();
  }

  loadInvitations() {
    this.isLoading = true;
    this.membersService.getMyInvitations().subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.invitations = Array.isArray(data) ? data : [];
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error fetching my invitations', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  acceptInvitation(invitationId: string, token: string) {
    this.membersService.acceptInvitation(invitationId, token).subscribe({
      next: () => {
        alert('Invitation accepted successfully!');
        // Reload organizations so the user can now access their new org
        this.orgCtx.myOrganization().subscribe(() => {
          this.router.navigate(['/org']);
        });
      },
      error: (err: any) => {
        console.error('Error accepting invitation', err);
        alert('Failed to accept invitation.');
      }
    });
  }

  logout() {
    localStorage.clear();
    this.router.navigate(['/auth/login']);
  }
}

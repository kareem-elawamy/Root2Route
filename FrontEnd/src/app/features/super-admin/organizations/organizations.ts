import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrganizationService } from './organization.service';
import { ToastService } from '../../../core/services/toast.service';
import { SkeletonComponent } from '../../../shared/components/skeleton/skeleton.component';

@Component({
  selector: 'app-organizations',
  standalone: true,
  imports: [CommonModule, FormsModule, SkeletonComponent],
  templateUrl: './organizations.html'
})
export class Organizations implements OnInit {
  private orgService = inject(OrganizationService);

  private toast = inject(ToastService);

  isLoading = signal(true);
  isDrawerOpen = false;
  selectedOrg: any = null;

  stats = {
    total: '0',
    pending: '0',
    suspended: '0'
  };

  organizations = signal<any[]>([]);

  // New state for approve/reject flow
  rejectionReason = signal('');
  isProcessing = signal(false);
  actionResult = signal<{type: 'success' | 'error', message: string} | null>(null);

  ngOnInit() {
    this.loadOrganizations();
  }

  loadOrganizations() {
    this.isLoading.set(true);
    this.orgService.getAllOrganizations().subscribe({
      next: (response: any) => {
        const rawData = response.data || response || [];
        const items = Array.isArray(rawData) ? rawData : [];

        const processed = items.map((org: any) => {
          const statusMap = ['Pending', 'Approved', 'Rejected', 'Suspended'];
          const typeMap = ['Farm', 'Restaurant', 'Factory', 'Tradesman'];
          
          const statusStr = statusMap[org.organizationStatus] || 'Unknown';
          let statusClass = 'bg-slate-100 text-slate-600';
          if (org.organizationStatus === 0) statusClass = 'bg-amber-100 text-amber-700'; // Pending
          if (org.organizationStatus === 1) statusClass = 'bg-emerald-100 text-emerald-700'; // Approved
          if (org.organizationStatus === 2) statusClass = 'bg-rose-100 text-rose-700'; // Rejected
          if (org.organizationStatus === 3) statusClass = 'bg-rose-100 text-rose-700'; // Suspended

          return {
            ...org,
            name: org.name || 'Unnamed Org',
            email: org.contactEmail || 'No email provided',
            type: typeMap[org.type] || 'Unknown',
            status: statusStr,
            statusValue: org.organizationStatus,
            statusClass: statusClass,
            initial: (org.name ? org.name.charAt(0) : '?').toUpperCase(),
            bgClass: 'bg-emerald-100 text-emerald-700',
            date: 'N/A', // Update if API returns created date
            details: {
              acreage: 'N/A',
              crop: 'N/A',
              desc: org.description || 'No description provided.'
            }
          };
        });

        this.organizations.set(processed);

        this.stats.total = this.organizations().length.toString();
        this.stats.pending = this.organizations().filter(org => org.statusValue === 0).length.toString();
        this.stats.suspended = this.organizations().filter(org => org.statusValue === 3).length.toString();

        this.isLoading.set(false);

      },
      error: (error: any) => {
        console.error('Error fetching organizations', error);
        this.isLoading.set(false);
      }
    });
  }

  openDetails(org: any) {
    this.selectedOrg = org;
    this.isDrawerOpen = true;
    this.rejectionReason.set('');
    this.actionResult.set(null);
  }

  closeDetails() {
    this.isDrawerOpen = false;
    this.selectedOrg = null;
    this.rejectionReason.set('');
    this.actionResult.set(null);
  }

  approveOrg() {
    if (!this.selectedOrg) return;
    
    this.isProcessing.set(true);
    this.actionResult.set(null);

    this.orgService.approveOrganization(this.selectedOrg.id).subscribe({
      next: () => {
        this.actionResult.set({ type: 'success', message: `Approved ${this.selectedOrg.name} successfully.` });
        this.loadOrganizations();
        setTimeout(() => {
          this.isProcessing.set(false);
          this.closeDetails();
        }, 1500);
      },
      error: (err: any) => {
        console.error(err);
        this.isProcessing.set(false);
        this.actionResult.set({ type: 'error', message: 'Error approving organization.' });
      }
    });
  }

  rejectOrg() {
    if (!this.selectedOrg) return;
    
    if (!this.rejectionReason().trim()) {
      this.actionResult.set({ type: 'error', message: 'Rejection reason is required.' });
      return;
    }

    this.isProcessing.set(true);
    this.actionResult.set(null);

    this.orgService.rejectOrganization(this.selectedOrg.id, this.rejectionReason()).subscribe({
      next: () => {
        this.actionResult.set({ type: 'success', message: `Rejected ${this.selectedOrg.name} successfully.` });
        this.loadOrganizations();
        setTimeout(() => {
          this.isProcessing.set(false);
          this.closeDetails();
        }, 1500);
      },
      error: (err: any) => {
        console.error(err);
        this.isProcessing.set(false);
        this.actionResult.set({ type: 'error', message: 'Error rejecting organization.' });
      }
    });
  }
}
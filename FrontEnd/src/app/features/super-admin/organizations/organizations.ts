import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrganizationService } from './organization.service';
import { ToastService } from '../../../core/services/toast.service';
import { SkeletonComponent } from '../../../shared/components/skeleton/skeleton.component';
import { StatChartComponent, ChartDataset } from '../../../shared/components/stat-chart/stat-chart.component';

@Component({
  selector: 'app-organizations',
  standalone: true,
  imports: [CommonModule, FormsModule, SkeletonComponent, StatChartComponent],
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
    rejected: '0'
  };

  organizations = signal<any[]>([]);

  // Filtering
  searchQuery = signal('');
  statusFilter = signal('All');

  filteredOrganizations = computed(() => {
    const query = this.searchQuery().toLowerCase();
    const status = this.statusFilter();
    
    return this.organizations().filter(org => {
      const matchesSearch = org.name.toLowerCase().includes(query) || org.email.toLowerCase().includes(query);
      const matchesStatus = status === 'All' || org.status === status;
      return matchesSearch && matchesStatus;
    });
  });

  // Chart Data (Status Distribution)
  chartData = computed(() => {
    const orgs = this.organizations();
    const total = orgs.length || 1; // Prevent div by 0
    
    const approved = orgs.filter(o => o.statusValue === 1).length;
    const pending = orgs.filter(o => o.statusValue === 0).length;
    const rejected = orgs.filter(o => o.statusValue === 2 || o.statusValue === 3).length;

    return {
      approved: { count: approved, percent: Math.round((approved / total) * 100) },
      pending: { count: pending, percent: Math.round((pending / total) * 100) },
      rejected: { count: rejected, percent: Math.round((rejected / total) * 100) },
    };
  });

  // Chart.js Doughnut Data
  orgChartLabels = ['Approved', 'Pending', 'Rejected / Suspended'];

  orgChartDatasets = computed((): ChartDataset[] => {
    const cd = this.chartData();
    return [
      {
        label: 'Organizations',
        data: [cd.approved.count, cd.pending.count, cd.rejected.count],
        backgroundColor: ['#10b981', '#f59e0b', '#ef4444'],
        borderColor: ['#ffffff', '#ffffff', '#ffffff'],
        borderWidth: 3,
      },
    ];
  });

  // New state for approve/reject flow
  rejectionReason = signal('');
  isProcessing = signal(false);
  orgStats = signal<any>(null);

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
            date: org.createdAt ? new Date(org.createdAt).toLocaleDateString() : 'N/A',
            complianceFileUrl: org.complianceFileUrl || null,
            rejectionReason: org.rejectionReason || null,
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
        this.stats.rejected = this.organizations().filter(org => org.statusValue === 2).length.toString();

        this.isLoading.set(false);

      },
      error: (error: any) => {
        console.error('Error fetching organizations', error);
        this.toast.error('Failed to load organizations.');
        this.isLoading.set(false);
      }
    });
  }

  openDetails(org: any) {
    this.selectedOrg = org;
    this.isDrawerOpen = true;
    this.rejectionReason.set('');
    this.orgStats.set(null);

    // Load organization statistics
    this.orgService.getOrgStatistics(org.id).subscribe({
      next: (response: any) => {
        const data = response.data || response;
        this.orgStats.set(data);
      },
      error: (err: any) => {
        console.error('Failed to load org statistics', err);
      }
    });
  }

  closeDetails() {
    this.isDrawerOpen = false;
    this.selectedOrg = null;
    this.rejectionReason.set('');
  }

  approveOrg() {
    if (!this.selectedOrg) return;

    this.isProcessing.set(true);

    // Use the dashboard approve endpoint for pending orgs, or the general status endpoint for re-approval
    const request$ = this.selectedOrg.statusValue === 0
      ? this.orgService.approveOrganization(this.selectedOrg.id)
      : this.orgService.updateStatus(this.selectedOrg.id, 1);

    request$.subscribe({
      next: () => {
        this.toast.success(`Approved ${this.selectedOrg.name} successfully.`);
        this.loadOrganizations();
        setTimeout(() => {
          this.isProcessing.set(false);
          this.closeDetails();
        }, 500);
      },
      error: (err: any) => {
        console.error(err);
        this.isProcessing.set(false);
        this.toast.error('Error approving organization.');
      }
    });
  }

  rejectOrg() {
    if (!this.selectedOrg) return;

    if (!this.rejectionReason().trim()) {
      this.toast.error('Rejection reason is required.');
      return;
    }

    this.isProcessing.set(true);

    // Use the dashboard reject endpoint for pending orgs, or the general status endpoint for re-rejection
    const request$ = this.selectedOrg.statusValue === 0
      ? this.orgService.rejectOrganization(this.selectedOrg.id, this.rejectionReason())
      : this.orgService.updateStatus(this.selectedOrg.id, 2);

    request$.subscribe({
      next: () => {
        this.toast.success(`Rejected ${this.selectedOrg.name} successfully.`);
        this.loadOrganizations();
        setTimeout(() => {
          this.isProcessing.set(false);
          this.closeDetails();
        }, 500);
      },
      error: (err: any) => {
        console.error(err);
        this.isProcessing.set(false);
        this.toast.error('Error rejecting organization.');
      }
    });
  }

  suspendOrg() {
    if (!this.selectedOrg) return;

    if (!this.rejectionReason().trim()) {
      this.toast.error('Suspension reason is required.');
      return;
    }

    this.isProcessing.set(true);

    this.orgService.updateStatus(this.selectedOrg.id, 3).subscribe({
      next: () => {
        this.toast.success(`Suspended ${this.selectedOrg.name} successfully.`);
        this.loadOrganizations();
        setTimeout(() => {
          this.isProcessing.set(false);
          this.closeDetails();
        }, 500);
      },
      error: (err: any) => {
        console.error(err);
        this.isProcessing.set(false);
        this.toast.error('Error suspending organization.');
      }
    });
  }
}
import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { OrganizationService } from './organization.service';

@Component({
  selector: 'app-organizations',
  standalone: true,
  templateUrl: './organizations.html'
})
export class Organizations implements OnInit {
  private orgService = inject(OrganizationService);
  private cdr = inject(ChangeDetectorRef); // 🟢 1. حقنّا "المنبه" بتاع الأنجولار

  isDrawerOpen = false;
  selectedOrg: any = null;

  stats = {
    total: '0',
    pending: '0',
    suspended: '0'
  };

  organizations: any[] = [];

  ngOnInit() {
    this.loadOrganizations();
  }

  loadOrganizations() {
    this.orgService.getAllOrganizations().subscribe({
      next: (response: any) => {
        const rawData = response.data || response || [];
        const items = Array.isArray(rawData) ? rawData : [];

        this.organizations = items.map((org: any) => {
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

        // 1. حساب الإجمالي
        this.stats.total = this.organizations.length.toString();

        // 2. فلترة وعد المنظمات اللي في الانتظار
        const pendingOrgs = this.organizations.filter(
          org => org.statusValue === 0
        );
        this.stats.pending = pendingOrgs.length.toString();

        // 3. فلترة وعد المنظمات الموقوفة
        const suspendedOrgs = this.organizations.filter(
          org => org.statusValue === 3
        );
        this.stats.suspended = suspendedOrgs.length.toString();

        console.log('All Organizations:', this.organizations);

        // 🟢 2. الضربة القاضية: بنقول للأنجولار يعرض الداتا الجديدة على الشاشة فوراً
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error fetching organizations', error);
      }
    });
  }

  openDetails(org: any) {
    this.selectedOrg = org;
    this.isDrawerOpen = true;
  }

  closeDetails() {
    this.isDrawerOpen = false;
    this.selectedOrg = null;
  }

  approveOrg() {
    if (!this.selectedOrg) return;
    this.orgService.updateStatus(this.selectedOrg.id, 1).subscribe({
      next: () => {
        alert(`Done: Approved ${this.selectedOrg.name}`);
        this.loadOrganizations();
        this.closeDetails();
      },
      error: (err: any) => {
        console.error(err);
        alert('Error approving organization');
      }
    });
  }

  rejectOrg() {
    if (!this.selectedOrg) return;
    this.orgService.updateStatus(this.selectedOrg.id, 2).subscribe({
      next: () => {
        alert(`Rejected ${this.selectedOrg.name}`);
        this.loadOrganizations();
        this.closeDetails();
      },
      error: (err: any) => {
        console.error(err);
        alert('Error rejecting organization');
      }
    });
  }
}
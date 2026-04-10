import { Component, OnInit, inject } from '@angular/core';
import { OrganizationService } from './organization.service';

@Component({
  selector: 'app-organizations',
  standalone: true, // ضفتلك دي عشان إحنا شغالين Standalone Architecture
  templateUrl: './organizations.html'
})
export class Organizations implements OnInit {
  private orgService = inject(OrganizationService);
  isDrawerOpen = false;
  selectedOrg: any = null;

  // خلينا الأرقام تبدأ بـ صفر لحد ما الداتا تيجي من الـ API
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
        this.organizations = response.data || [];
        
        // 1. حساب الإجمالي
        this.stats.total = this.organizations.length.toString();

        // 2. فلترة وعد المنظمات اللي في الانتظار (Pending)
        // ملاحظة: اتأكد من كلمة 'Pending' هل الباك إند بيبعتها كلمة ولا رقم (زي 0 مثلاً)
        const pendingOrgs = this.organizations.filter(
          org => org.status === 'Pending' || org.status === 0
        );
        this.stats.pending = pendingOrgs.length.toString();

        // 3. فلترة وعد المنظمات الموقوفة (Suspended)
        const suspendedOrgs = this.organizations.filter(
          org => org.status === 'Suspended' || org.status === 2
        );
        this.stats.suspended = suspendedOrgs.length.toString();

        console.log('All Organizations:', this.organizations);
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
    // هنا هنبقى نربطها بـ API الـ Approve
    alert(`Done: Approved ${this.selectedOrg.name}`);
    this.closeDetails();
  }

  rejectOrg() {
     // هنا هنبقى نربطها بـ API الـ Reject
    alert(`Rejected ${this.selectedOrg.name}`);
    this.closeDetails();
  }
}
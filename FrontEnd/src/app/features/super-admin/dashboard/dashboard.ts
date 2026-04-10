import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService } from './dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html'
})
export class Dashboard implements OnInit {
  private dashboardService = inject(DashboardService);

  overviewStats: any;
  // مسحنا الداتا الوهمية وخليناه فاضي في الأول
  organizations: any[] = [];

  ngOnInit() {
    this.fetchOverviewStats();
    this.fetchPendingOrganizations();
  }

  fetchOverviewStats() {
    this.dashboardService.getOverviewStats().subscribe({
      next: (response: any) => {
        this.overviewStats = response.data;
      },
      error: (error: any) => {
        console.error('Error fetching overview stats', error);
      }
    });
  }

  fetchPendingOrganizations() {
    this.dashboardService.getPendingOrganizations().subscribe({
      next: (response: any) => {
        // بنحط الداتا اللي راجعة في المتغير بتاعنا
        // فرضنا إن الباك إند بيبعت لستة المنظمات جوه response.data برضو
        this.organizations = response.data || [];
        console.log('Pending Orgs:', this.organizations); // عشان نشوف شكل الداتا
      },
      error: (error: any) => {
        console.error('Error fetching pending orgs', error);
      }
    });
  }
}
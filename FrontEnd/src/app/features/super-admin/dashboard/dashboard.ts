import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DashboardService } from './dashboard.service';
import { SkeletonComponent } from '../../../shared/components/skeleton/skeleton.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, SkeletonComponent],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  private dashboardService = inject(DashboardService);

  // State signals
  isLoading = signal(true);
  overviewStats = signal({
    grossRevenue: 0,
    platformFees: 0,
    pendingOrganizations: 0
  });

  organizations = signal<any[]>([]);
  financials = signal<any[]>([]);
  pendingProducts = signal<any[]>([]);

  ngOnInit() {
    this.fetchOverviewStats();
    this.fetchPendingOrganizations();
    this.fetchFinancials();
    this.fetchPendingProducts();
    
    setTimeout(() => this.isLoading.set(false), 800);
  }

  fetchOverviewStats() {
    this.dashboardService.getOverviewStats().subscribe({
      next: (response: any) => {
        const actualData = response.data || response;
        if (actualData) {
          this.overviewStats.set({
            grossRevenue: actualData.grossRevenue || 0,
            platformFees: actualData.platformFees || 0,
            pendingOrganizations: actualData.pendingOrganizations || 0
          });
        }
      },
      error: (error: any) => console.error('Error fetching overview stats', error)
    });
  }

  fetchPendingOrganizations() {
    this.dashboardService.getPendingOrganizations().subscribe({
      next: (response: any) => {
        const data = response.data || response;
        this.organizations.set(Array.isArray(data) ? data : []);
      },
      error: (error: any) => console.error('Error fetching pending orgs', error)
    });
  }

  fetchFinancials() {
    this.dashboardService.getFinancials().subscribe({
      next: (response: any) => {
        const data = response.data || response;
        this.financials.set(Array.isArray(data) ? data : []);
      },
      error: (error: any) => console.error('Error fetching financials', error)
    });
  }



  fetchPendingProducts() {
    this.dashboardService.getPendingProducts().subscribe({
      next: (response: any) => {
        const data = response.data || response;
        this.pendingProducts.set(Array.isArray(data) ? data : []);
      },
      error: (error: any) => console.error('Error fetching pending products', error)
    });
  }


}
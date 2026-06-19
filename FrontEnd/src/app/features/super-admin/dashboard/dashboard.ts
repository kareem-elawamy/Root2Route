import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DashboardService } from './dashboard.service';
import { SkeletonComponent } from '../../../shared/components/skeleton/skeleton.component';
import { StatChartComponent, ChartDataset } from '../../../shared/components/stat-chart/stat-chart.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, SkeletonComponent, StatChartComponent],
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

  // ── Chart Computed Signals ──
  revenueChartLabels = computed(() => this.financials().map(f => f.month));

  revenueChartDatasets = computed((): ChartDataset[] => {
    const data = this.financials();
    return [
      {
        label: 'Gross Revenue',
        data: data.map(f => f.grossRevenue || 0),
        borderColor: '#10b981',
        backgroundColor: 'rgba(16, 185, 129, 0.08)',
        fill: true,
        tension: 0.4,
        pointRadius: 5,
        pointHoverRadius: 8,
      },
      {
        label: 'Platform Fees',
        data: data.map(f => f.platformFees || 0),
        borderColor: '#6366f1',
        backgroundColor: 'rgba(99, 102, 241, 0.08)',
        fill: true,
        tension: 0.4,
        pointRadius: 5,
        pointHoverRadius: 8,
      },
    ];
  });

  ngOnInit() {
    this.fetchOverviewStats();
    this.fetchPendingOrganizations();
    this.fetchFinancials();
    this.fetchPendingProducts();
    this.fetchTopDiseases();
    this.fetchAccuracyTrend();
    this.fetchDiseaseHeatmap();
    
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

  // ── ML Data ──
  topDiseases = signal<any[]>([]);
  accuracyTrend = signal<any[]>([]);
  diseaseHeatmap = signal<any[]>([]);

  fetchTopDiseases() {
    this.dashboardService.getTopDiseases().subscribe({
      next: (response: any) => {
        const data = response.data || response;
        this.topDiseases.set(Array.isArray(data) ? data : []);
      },
      error: (error: any) => console.error('Error fetching top diseases', error)
    });
  }

  fetchAccuracyTrend() {
    this.dashboardService.getMLAccuracyTrend().subscribe({
      next: (response: any) => {
        const data = response.data || response;
        this.accuracyTrend.set(Array.isArray(data) ? data : []);
      },
      error: (error: any) => console.error('Error fetching accuracy trend', error)
    });
  }

  fetchDiseaseHeatmap() {
    this.dashboardService.getDiseaseHeatmap().subscribe({
      next: (response: any) => {
        const data = response.data || response;
        this.diseaseHeatmap.set(Array.isArray(data) ? data : []);
      },
      error: (error: any) => console.error('Error fetching disease heatmap', error)
    });
  }
}
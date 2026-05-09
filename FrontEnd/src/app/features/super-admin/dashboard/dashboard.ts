import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
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
  private cdr = inject(ChangeDetectorRef); // 🟢 "المنبه" اللي بيفوق الشاشة

  // 1. بندي قيم مبدئية عشان الـ HTML ميضربش إيرور
  overviewStats = {
    grossRevenue: 0,
    platformFees: 0,
    pendingOrganizations: 0,
    totalMLDiagnoses: 0
  };

  organizations: any[] = []; // مصفوفة فاضية لحد ما الداتا تيجي

  ngOnInit() {
    this.fetchOverviewStats();
    this.fetchPendingOrganizations();
  }

  fetchOverviewStats() {
    this.dashboardService.getOverviewStats().subscribe({
      next: (response: any) => {
        // بنقرأ الداتا من الأوبجيكت اللي الـ .NET بيرجعه
        const actualData = response.data || response;

        if (actualData) {
          this.overviewStats.grossRevenue = actualData.grossRevenue || 0;
          this.overviewStats.platformFees = actualData.platformFees || 0;
          this.overviewStats.pendingOrganizations = actualData.pendingOrganizations || 0;
          this.overviewStats.totalMLDiagnoses = actualData.totalMLDiagnoses || 0;
        }

        // 🟢 إجبار الأنجولار يعرض الداتا الجديدة فوراً
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error fetching overview stats', error);
      }
    });
  }

  fetchPendingOrganizations() {
    this.dashboardService.getPendingOrganizations().subscribe({
      next: (response: any) => {
        this.organizations = response.data || [];

        // 🟢 إجبار الأنجولار يعرض لستة الشركات فوراً
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error fetching pending orgs', error);
      }
    });
  }
}
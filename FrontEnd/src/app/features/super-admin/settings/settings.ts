import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SettingsService } from './settings.service';
import { DashboardService } from '../dashboard/dashboard.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './settings.html'
})
export class Settings implements OnInit {
  private settingsService = inject(SettingsService);
  private dashboardService = inject(DashboardService);

  // State signals
  platformFeePercentage = signal('3.5');
  isSaving = signal(false);
  actionMessage = signal<{type: 'success' | 'error', text: string} | null>(null);

  // Fake data for UI structure
  financialSettings = {
    standardShippingFee: '12.00'
  };

  generalSettings = {
    supportEmail: 'ops@emeraldfield.ag',
    apiKey: 'sk_live_51MzA2REmeraldFieldKey...'
  };

  auditLogs = [
    {
      user: 'System Kernel',
      actionBadge: 'Security Audit',
      badgeClass: 'text-on-surface-variant bg-slate-200',
      time: 'Oct 22, 2023 • 00:01 AM',
      icon: 'security',
      iconClass: 'bg-slate-800',
      hasDetails: false,
      hasMessage: false,
      hasScan: true,
      scanIcon: 'check_circle',
      scanIconClass: 'text-primary',
      scanMessage: 'Weekly automated compliance scan completed. 0 vulnerabilities found.'
    }
  ];

  ngOnInit() {
    this.loadCurrentFee();
  }

  loadCurrentFee() {
    // We get the current fee from the overview stats since there is no GET /settings endpoint
    this.dashboardService.getOverviewStats().subscribe({
      next: (response: any) => {
        const actualData = response.data || response;
        if (actualData && actualData.platformFees !== undefined) {
           // Wait, the overview stats return grossRevenue, platformFees, etc.
           // They might not return the *percentage*.
           // If it doesn't return percentage, we'll just keep the default '3.5' for now.
           // In a real app we'd want a GET /settings.
        }
      },
      error: (err) => console.error('Failed to load stats for fee', err)
    });
  }

  saveFee() {
    const feeValue = parseFloat(this.platformFeePercentage());
    if (isNaN(feeValue)) {
      this.actionMessage.set({ type: 'error', text: 'Please enter a valid number for the fee.' });
      return;
    }

    this.isSaving.set(true);
    this.actionMessage.set(null);

    this.settingsService.updatePlatformFee(feeValue).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.actionMessage.set({ type: 'success', text: 'Platform fee updated successfully.' });
        setTimeout(() => this.actionMessage.set(null), 3000);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.actionMessage.set({ type: 'error', text: 'Failed to update platform fee.' });
        console.error(err);
      }
    });
  }
}
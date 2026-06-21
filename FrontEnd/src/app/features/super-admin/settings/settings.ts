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

  // Tab navigation
  activeTab = signal<'financials' | 'general'>('financials');

  // State signals
  platformFeePercentage = signal('3.5');
  isSaving = signal(false);
  actionMessage = signal<{type: 'success' | 'error', text: string} | null>(null);

  financialSettings = {
    standardShippingFee: '12.00'
  };

  ngOnInit() {
    this.loadCurrentFee();
  }

  switchTab(tab: 'financials' | 'general') {
    this.activeTab.set(tab);
  }

  loadCurrentFee() {
    this.dashboardService.getOverviewStats().subscribe({
      next: (response: any) => {
        const actualData = response.data || response;
        if (actualData && actualData.platformFeePercentage !== undefined) {
          this.platformFeePercentage.set(actualData.platformFeePercentage.toString());
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
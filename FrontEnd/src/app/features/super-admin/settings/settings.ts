import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SettingsService } from './settings.service';
import { DashboardService } from '../dashboard/dashboard.service';
import { AuditLogService } from '../../../core/services/audit-log.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './settings.html'
})
export class Settings implements OnInit {
  private settingsService = inject(SettingsService);
  private dashboardService = inject(DashboardService);
  private auditLogService = inject(AuditLogService);

  // Tab navigation
  activeTab = signal<'financials' | 'general' | 'auditLogs'>('financials');

  // State signals
  platformFeePercentage = signal('3.5');
  isSaving = signal(false);
  actionMessage = signal<{type: 'success' | 'error', text: string} | null>(null);

  financialSettings = {
    standardShippingFee: '12.00'
  };

  // Audit logs
  auditLogs = signal<any[]>([]);
  isLoadingLogs = signal(false);

  ngOnInit() {
    this.loadCurrentFee();
  }

  switchTab(tab: 'financials' | 'general' | 'auditLogs') {
    this.activeTab.set(tab);
    if (tab === 'auditLogs' && this.auditLogs().length === 0) {
      this.loadAuditLogs();
    }
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

  loadAuditLogs() {
    this.isLoadingLogs.set(true);
    this.auditLogService.getAuditLogs(1, 10).subscribe({
      next: (response: any) => {
        const data = response.data || response;
        const items = data.items || data.logs || (Array.isArray(data) ? data : []);
        this.auditLogs.set(items);
        this.isLoadingLogs.set(false);
      },
      error: (err) => {
        console.error('Failed to load audit logs', err);
        this.isLoadingLogs.set(false);
      }
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

  getActionIcon(action: string): string {
    if (action === 'Create') return 'add_circle';
    if (action === 'Update') return 'edit';
    if (action === 'Delete') return 'delete';
    return 'info';
  }

  getActionClass(action: string): string {
    if (action === 'Create') return 'bg-emerald-100 text-emerald-700';
    if (action === 'Update') return 'bg-amber-100 text-amber-700';
    if (action === 'Delete') return 'bg-rose-100 text-rose-700';
    return 'bg-slate-100 text-slate-700';
  }
}
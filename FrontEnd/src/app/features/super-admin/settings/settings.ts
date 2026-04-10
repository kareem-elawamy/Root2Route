import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './settings.html'
})
export class Settings {
  financialSettings = {
    platformFeePercentage: '3.5',
    standardShippingFee: '12.00'
  };

  generalSettings = {
    supportEmail: 'ops@emeraldfield.ag',
    apiKey: 'sk_live_51MzA2REmeraldFieldKey...'
  };

  auditLogs = [
    {
      user: 'Marcus Sterling',
      actionBadge: 'Updated Fees',
      badgeClass: 'text-primary bg-primary/5',
      time: 'Oct 24, 2023 • 14:22 PM',
      icon: 'update',
      iconClass: 'bg-primary',
      hasDetails: true,
      oldValue: '3.2%',
      newValue: '3.5%',
      hasMessage: false,
      hasScan: false
    },
    {
      user: 'Sarah Chen',
      actionBadge: 'Purged Keys',
      badgeClass: 'text-tertiary bg-tertiary/5',
      time: 'Oct 23, 2023 • 09:15 AM',
      icon: 'delete',
      iconClass: 'bg-tertiary',
      hasDetails: false,
      hasMessage: true,
      message: 'Removed expired API keys for deprecated legacy inventory system.',
      hasScan: false
    },
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
}
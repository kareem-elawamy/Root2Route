import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WithdrawalService } from '../../../../core/services/withdrawal.service';
import { ToastService } from '../../../../core/services/toast.service';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { PaytabsService } from '../../../../core/services/paytabs.service';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartOptions } from 'chart.js';

@Component({
  selector: 'app-wallet',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective],
  templateUrl: './wallet.html',
  styleUrls: ['./wallet.css']
})
export class WalletComponent implements OnInit {
  private withdrawalService = inject(WithdrawalService);
  private paytabsService = inject(PaytabsService);
  private toastService = inject(ToastService);
  public orgContext = inject(OrgContextService);

  withdrawals: any[] = [];
  isLoading = true;
  isRequesting = false;

  // Analytics
  analyticsData: any = null;
  public lineChartData: ChartConfiguration<'line'>['data'] = {
    labels: [],
    datasets: []
  };
  public lineChartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false }
    }
  };

  // Withdrawal Form
  amountToWithdraw: number = 0;
  bankName: string = '';
  accountName: string = '';
  accountNumber: string = '';
  swiftCode: string = '';

  get activeOrg() {
    return this.orgContext.activeOrg();
  }

  ngOnInit(): void {
    this.loadWithdrawals();
    this.loadAnalytics();
  }

  loadWithdrawals() {
    this.isLoading = true;
    this.withdrawalService.getOrganizationWithdrawals().subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.withdrawals = res.data;
        } else {
          this.toastService.error(res.message || 'Failed to load withdrawals');
        }
        this.isLoading = false;
      },
      error: () => {
        this.toastService.error('Failed to fetch withdrawals');
        this.isLoading = false;
      }
    });
  }

  loadAnalytics() {
    const from = new Date();
    from.setMonth(from.getMonth() - 1);
    const to = new Date();

    this.paytabsService.getOrganizationAnalytics(from.toISOString(), to.toISOString()).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.analyticsData = res.data;
          // Build chart data
          if (this.analyticsData.dailyBreakdown && this.analyticsData.dailyBreakdown.length > 0) {
            this.lineChartData = {
              labels: this.analyticsData.dailyBreakdown.map((s: any) => s.date),
              datasets: [
                {
                  data: this.analyticsData.dailyBreakdown.map((s: any) => s.revenue),
                  label: 'Revenue (EGP)',
                  fill: true,
                  tension: 0.4,
                  borderColor: '#10b981',
                  backgroundColor: 'rgba(16, 185, 129, 0.2)'
                }
              ]
            };
          }
        }
      }
    });
  }

  requestWithdrawal() {
    if (this.amountToWithdraw <= 0) {
      this.toastService.error('Amount must be greater than 0');
      return;
    }

    if (!this.bankName || !this.accountName || !this.accountNumber) {
      this.toastService.error('Please fill all bank details');
      return;
    }

    this.isRequesting = true;
    const payload = {
      amount: this.amountToWithdraw,
      bankName: this.bankName,
      accountName: this.accountName,
      accountNumber: this.accountNumber,
      swiftCode: this.swiftCode
    };

    this.withdrawalService.requestWithdrawal(payload).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.toastService.success('Withdrawal requested successfully');
          this.amountToWithdraw = 0;
          this.loadWithdrawals(); // Reload list
        } else {
          this.toastService.error(res.message || 'Error occurred');
        }
        this.isRequesting = false;
      },
      error: () => {
        this.toastService.error('Could not request withdrawal');
        this.isRequesting = false;
      }
    });
  }
}

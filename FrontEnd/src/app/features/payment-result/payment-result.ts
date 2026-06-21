import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { PaytabsService } from '../../core/services/paytabs.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-payment-result',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './payment-result.html',
  styleUrls: ['./payment-result.css']
})
export class PaymentResultComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private paytabsService = inject(PaytabsService);
  private toastService = inject(ToastService);

  isLoading = true;
  isSuccess = false;
  paymentData: any = null;
  errorMessage = '';

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const tranRef = params['tranRef'];
      if (tranRef) {
        this.verifyPayment(tranRef);
      } else {
        this.isLoading = false;
        this.errorMessage = 'No transaction reference found.';
      }
    });
  }

  verifyPayment(tranRef: string) {
    this.paytabsService.verifyPayment(tranRef).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.succeeded && res.data) {
          this.paymentData = res.data;
          this.isSuccess = res.data.status === 'Captured' || res.data.status === 'Paid';
        } else {
          this.errorMessage = res.message || 'Payment verification failed.';
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'An error occurred while verifying the payment.';
        console.error(err);
      }
    });
  }
}

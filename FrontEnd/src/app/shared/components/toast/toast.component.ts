import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService, ToastType } from '../../../core/services/toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="toast-container">
      @for (toast of toastService.toasts(); track toast.id) {
        <div class="toast" [ngClass]="getToastClass(toast.type)">
          <div class="toast-icon">
            <span class="material-symbols-outlined">{{ getIcon(toast.type) }}</span>
          </div>
          <div class="toast-content">
            <p>{{ toast.message }}</p>
          </div>
          <button class="toast-close" (click)="toastService.removeToast(toast.id)">
            <span class="material-symbols-outlined">close</span>
          </button>
        </div>
      }
    </div>
  `,
  styleUrl: './toast.component.css'
})
export class ToastComponent {
  public toastService = inject(ToastService);

  getToastClass(type: ToastType): string {
    return `toast-${type}`;
  }

  getIcon(type: ToastType): string {
    switch (type) {
      case 'success': return 'check_circle';
      case 'error': return 'error';
      case 'warning': return 'warning';
      case 'info': return 'info';
    }
  }
}

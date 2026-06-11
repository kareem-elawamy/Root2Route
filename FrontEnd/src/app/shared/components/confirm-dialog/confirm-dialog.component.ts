import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfirmDialogService } from '../../../core/services/confirm-dialog.service';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (dialogService.options(); as options) {
      <div class="dialog-overlay" (click)="dialogService.cancel()">
        <div class="dialog-container" (click)="$event.stopPropagation()">
          <div class="dialog-header" [ngClass]="{'destructive': options.isDestructive}">
            <div class="dialog-icon">
              <span class="material-symbols-outlined">{{ options.isDestructive ? 'warning' : 'help' }}</span>
            </div>
            <h3 class="dialog-title">{{ options.title }}</h3>
          </div>
          
          <div class="dialog-body">
            <p>{{ options.message }}</p>
          </div>
          
          <div class="dialog-actions">
            <button class="btn-cancel" (click)="dialogService.cancel()">
              {{ options.cancelLabel }}
            </button>
            <button class="btn-confirm" [ngClass]="{'btn-destructive': options.isDestructive}" (click)="dialogService.confirm()">
              {{ options.confirmLabel }}
            </button>
          </div>
        </div>
      </div>
    }
  `,
  styleUrl: './confirm-dialog.component.css'
})
export class ConfirmDialogComponent {
  public dialogService = inject(ConfirmDialogService);
}

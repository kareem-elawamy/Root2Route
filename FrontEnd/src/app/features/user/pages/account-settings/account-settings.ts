import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { ConfirmDialogService } from '../../../../core/services/confirm-dialog.service';

@Component({
  selector: 'app-account-settings',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page-container">
      <h1 class="page-title">Account Settings</h1>

      <div class="settings-card">
        <div class="settings-section">
          <h2>Security</h2>
          <div class="setting-row">
            <div>
              <h3>Change Password</h3>
              <p>Update your account password regularly for better security.</p>
            </div>
            <a routerLink="/user/change-password" class="action-btn">Change Password</a>
          </div>
        </div>

        <div class="divider"></div>

        <div class="settings-section danger-zone">
          <h2>Danger Zone</h2>
          <div class="setting-row">
            <div>
              <h3>Delete Account</h3>
              <p>Permanently delete your account and all associated data. This action cannot be undone.</p>
            </div>
            <button class="danger-btn" (click)="deleteAccount()" [disabled]="isDeleting()">
              @if (isDeleting()) { Deleting... } @else { Delete Account }
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 2rem; max-width: 720px; margin: 0 auto; }
    .page-title { font-size: 1.5rem; font-weight: 800; color: #0f172a; margin-bottom: 1.5rem; }
    .settings-card { background: white; border-radius: 1.25rem; padding: 2rem; box-shadow: 0 4px 24px rgba(0,0,0,.06); }
    .settings-section h2 { font-size: .875rem; font-weight: 700; color: #64748b; text-transform: uppercase; letter-spacing: .05em; margin: 0 0 1.25rem; }
    .setting-row { display: flex; align-items: center; justify-content: space-between; gap: 1.5rem; }
    .setting-row h3 { font-size: 1rem; font-weight: 700; color: #0f172a; margin: 0 0 .25rem; }
    .setting-row p { font-size: .8125rem; color: #64748b; margin: 0; }
    .action-btn { padding: .625rem 1.25rem; background: #10b981; color: white; border: none; border-radius: .5rem; font-weight: 700; font-size: .8125rem; cursor: pointer; text-decoration: none; white-space: nowrap; }
    .action-btn:hover { background: #059669; }
    .divider { height: 1px; background: #e2e8f0; margin: 1.5rem 0; }
    .danger-zone h2 { color: #dc2626; }
    .danger-btn { padding: .625rem 1.25rem; background: #fef2f2; color: #dc2626; border: 2px solid #fecaca; border-radius: .5rem; font-weight: 700; font-size: .8125rem; cursor: pointer; white-space: nowrap; transition: all .2s; }
    .danger-btn:hover { background: #dc2626; color: white; border-color: #dc2626; }
    .danger-btn:disabled { opacity: .5; cursor: not-allowed; }
  `]
})
export class AccountSettingsComponent {
  private auth = inject(AuthService);
  private router = inject(Router);
  private toast = inject(ToastService);
  private confirm = inject(ConfirmDialogService);

  isDeleting = signal(false);

  async deleteAccount() {
    const confirmed = await this.confirm.open({
      title: 'Delete Account',
      message: 'This will permanently delete your account. All your data will be lost. Are you absolutely sure?',
      confirmLabel: 'Yes, Delete My Account',
    });
    if (!confirmed) return;

    this.isDeleting.set(true);
    this.auth.deleteAccount().subscribe({
      next: () => {
        this.isDeleting.set(false);
        this.auth.clearSession();
        this.toast.success('Account deleted.');
        this.router.navigate(['/auth/login']);
      },
      error: (err) => {
        this.isDeleting.set(false);
        this.toast.error(err.message || 'Failed to delete account.');
      }
    });
  }
}

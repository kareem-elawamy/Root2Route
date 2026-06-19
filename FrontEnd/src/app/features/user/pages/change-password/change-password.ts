import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page-container">
      <div class="page-card">
        <div class="page-header">
          <span class="material-symbols-outlined header-icon">lock</span>
          <h1>Change Password</h1>
          <p>Update your account password. You'll need your current password to confirm.</p>
        </div>

        <form (ngSubmit)="submit()">
          <div class="form-group">
            <label>Current Password</label>
            <input type="password" [(ngModel)]="oldPassword" name="oldPassword" placeholder="••••••••" required />
          </div>
          <div class="form-group">
            <label>New Password</label>
            <input type="password" [(ngModel)]="newPassword" name="newPassword" placeholder="••••••••" required />
          </div>
          <div class="form-group">
            <label>Confirm New Password</label>
            <input type="password" [(ngModel)]="confirmPassword" name="confirmPassword" placeholder="••••••••" required />
          </div>
          @if (error()) {
            <div class="error-box">{{ error() }}</div>
          }
          <button class="submit-btn" type="submit" [disabled]="isLoading()">
            @if (isLoading()) { <span class="spinner"></span> Updating... } @else { Update Password }
          </button>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 2rem; max-width: 520px; margin: 0 auto; }
    .page-card { background: white; border-radius: 1.25rem; padding: 2.5rem; box-shadow: 0 4px 24px rgba(0,0,0,.06); }
    .page-header { margin-bottom: 2rem; }
    .header-icon { font-size: 2.5rem; color: #10b981; display: block; margin-bottom: .5rem; }
    .page-header h1 { font-size: 1.375rem; font-weight: 800; color: #0f172a; margin: 0 0 .5rem; }
    .page-header p { color: #64748b; font-size: .8125rem; margin: 0; }
    .form-group { margin-bottom: 1.25rem; }
    .form-group label { display: block; font-size: .75rem; font-weight: 700; color: #475569; text-transform: uppercase; letter-spacing: .05em; margin-bottom: .5rem; }
    .form-group input { width: 100%; padding: .875rem 1rem; border: 2px solid #e2e8f0; border-radius: .75rem; font-size: .9375rem; box-sizing: border-box; transition: border-color .2s; }
    .form-group input:focus { outline: none; border-color: #10b981; }
    .submit-btn { width: 100%; padding: 1rem; background: #10b981; color: white; border: none; border-radius: .75rem; font-size: 1rem; font-weight: 700; cursor: pointer; display: flex; align-items: center; justify-content: center; gap: .5rem; transition: background .2s; }
    .submit-btn:hover { background: #059669; }
    .submit-btn:disabled { opacity: .6; cursor: not-allowed; }
    .error-box { background: #fef2f2; color: #dc2626; padding: .75rem 1rem; border-radius: .5rem; font-size: .8125rem; font-weight: 600; margin-bottom: 1rem; }
    .spinner { width: 1.25rem; height: 1.25rem; border: 2px solid white; border-top-color: transparent; border-radius: 50%; animation: spin .6s linear infinite; }
    @keyframes spin { to { transform: rotate(360deg); } }
  `]
})
export class ChangePasswordComponent {
  private auth = inject(AuthService);
  private router = inject(Router);
  private toast = inject(ToastService);

  oldPassword = '';
  newPassword = '';
  confirmPassword = '';
  isLoading = signal(false);
  error = signal('');

  submit() {
    if (!this.oldPassword || !this.newPassword || !this.confirmPassword) {
      this.error.set('All fields are required.');
      return;
    }
    if (this.newPassword !== this.confirmPassword) {
      this.error.set('New passwords do not match.');
      return;
    }
    this.isLoading.set(true);
    this.error.set('');

    this.auth.changePassword({ oldPassword: this.oldPassword, newPassword: this.newPassword }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.toast.success('Password changed successfully!');
        this.oldPassword = ''; this.newPassword = ''; this.confirmPassword = '';
      },
      error: (err) => { this.isLoading.set(false); this.error.set(err.message || 'Failed.'); }
    });
  }
}

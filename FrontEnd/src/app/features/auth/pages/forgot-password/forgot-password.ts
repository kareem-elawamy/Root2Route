import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="auth-container">
      <div class="auth-card">
        <div class="auth-header">
          <span class="material-symbols-outlined auth-icon">lock_reset</span>
          <h1>Forgot Password</h1>
          <p>Enter your email address and we'll send you an OTP to reset your password.</p>
        </div>

        @if (success()) {
          <div class="success-box">
            <span class="material-symbols-outlined">check_circle</span>
            <p>OTP sent successfully! Check your email.</p>
          </div>
          <button class="auth-btn" routerLink="/auth/reset-password" [queryParams]="{email: email}">
            Continue to Reset Password
          </button>
        } @else {
          <form (ngSubmit)="submit()">
            <div class="form-group">
              <label>Email Address</label>
              <input type="email" [(ngModel)]="email" name="email" placeholder="you@example.com" required />
            </div>
            @if (error()) {
              <div class="error-box">{{ error() }}</div>
            }
            <button class="auth-btn" type="submit" [disabled]="isLoading()">
              @if (isLoading()) { <span class="spinner"></span> Sending... } @else { Send Reset OTP }
            </button>
          </form>
        }

        <a routerLink="/auth/login" class="auth-link">← Back to Login</a>
      </div>
    </div>
  `,
  styles: [`
    .auth-container { min-height: 100vh; display: flex; align-items: center; justify-content: center; background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%); padding: 2rem; }
    .auth-card { background: white; border-radius: 1.5rem; padding: 3rem; max-width: 440px; width: 100%; box-shadow: 0 25px 50px -12px rgba(0,0,0,.25); }
    .auth-header { text-align: center; margin-bottom: 2rem; }
    .auth-icon { font-size: 3rem; color: #10b981; margin-bottom: .5rem; display: block; }
    .auth-header h1 { font-size: 1.5rem; font-weight: 800; color: #0f172a; margin: .5rem 0; }
    .auth-header p { color: #64748b; font-size: .875rem; }
    .form-group { margin-bottom: 1.25rem; }
    .form-group label { display: block; font-size: .75rem; font-weight: 700; color: #475569; text-transform: uppercase; letter-spacing: .05em; margin-bottom: .5rem; }
    .form-group input { width: 100%; padding: .875rem 1rem; border: 2px solid #e2e8f0; border-radius: .75rem; font-size: .9375rem; transition: border-color .2s; box-sizing: border-box; }
    .form-group input:focus { outline: none; border-color: #10b981; }
    .auth-btn { width: 100%; padding: 1rem; background: #10b981; color: white; border: none; border-radius: .75rem; font-size: 1rem; font-weight: 700; cursor: pointer; transition: background .2s; display: flex; align-items: center; justify-content: center; gap: .5rem; }
    .auth-btn:hover { background: #059669; }
    .auth-btn:disabled { opacity: .6; cursor: not-allowed; }
    .auth-link { display: block; text-align: center; margin-top: 1.5rem; color: #10b981; font-weight: 600; font-size: .875rem; text-decoration: none; }
    .error-box { background: #fef2f2; color: #dc2626; padding: .75rem 1rem; border-radius: .5rem; font-size: .8125rem; font-weight: 600; margin-bottom: 1rem; }
    .success-box { background: #f0fdf4; color: #16a34a; padding: 1rem; border-radius: .75rem; font-size: .875rem; font-weight: 600; margin-bottom: 1.5rem; display: flex; align-items: center; gap: .5rem; }
    .spinner { width: 1.25rem; height: 1.25rem; border: 2px solid white; border-top-color: transparent; border-radius: 50%; animation: spin .6s linear infinite; }
    @keyframes spin { to { transform: rotate(360deg); } }
  `]
})
export class ForgotPasswordComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  email = '';
  isLoading = signal(false);
  error = signal('');
  success = signal(false);

  submit() {
    if (!this.email) { this.error.set('Please enter your email.'); return; }
    this.isLoading.set(true);
    this.error.set('');

    this.auth.forgotPassword(this.email).subscribe({
      next: () => { this.isLoading.set(false); this.success.set(true); },
      error: (err) => { this.isLoading.set(false); this.error.set(err.message || 'Failed to send OTP.'); }
    });
  }
}

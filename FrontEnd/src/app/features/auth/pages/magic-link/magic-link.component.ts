// import { Component, OnInit, inject, signal } from '@angular/core';
// import { ActivatedRoute, Router } from '@angular/router';
// import { HttpClient } from '@angular/common/http';
// import { AuthService } from '../../../../core/services/auth.service';
// import { OrgContextService, Organization } from '../../../../core/services/org-context.service';

// type MagicLinkState = 'verifying' | 'success' | 'error' | 'no-token';

// interface MagicLinkResponse {
//   token: string;
//   user: {
//     id: string;
//     email: string;
//     name: string;
//     permissions: string[];
//   };
//   organizations: Organization[];
// }

// @Component({
//   selector: 'app-magic-link',
//   standalone: true,
//   imports: [],
//   template: `
//     <div class="magic-page">
//       <!-- Background -->
//       <div class="orb orb-1"></div>
//       <div class="orb orb-2"></div>

//       <div class="magic-card">
//         <!-- Brand -->
//         <div class="brand">
//           <div class="brand-icon">
//             <svg width="32" height="32" viewBox="0 0 32 32" fill="none">
//               <path d="M16 2L2 10V22L16 30L30 22V10L16 2Z" stroke="url(#grad2)" stroke-width="2" fill="none"/>
//               <path d="M16 8L8 13V19L16 24L24 19V13L16 8Z" fill="url(#grad2)" opacity="0.6"/>
//               <defs>
//                 <linearGradient id="grad2" x1="2" y1="2" x2="30" y2="30">
//                   <stop offset="0%" stop-color="#6366f1"/>
//                   <stop offset="100%" stop-color="#8b5cf6"/>
//                 </linearGradient>
//               </defs>
//             </svg>
//           </div>
//           <span class="brand-name">Root2Route</span>
//         </div>

//         <!-- ── VERIFYING State ── -->
//         @if (state() === 'verifying') {
//           <div class="state-block" id="state-verifying">
//             <div class="pulse-ring">
//               <div class="pulse-dot"></div>
//             </div>
//             <h2>جاري التحقق من الرابط</h2>
//             <p>لحظة واحدة، نقوم بالتحقق من هويتك...</p>
//             <div class="progress-bar"><div class="progress-fill"></div></div>
//           </div>
//         }

//         <!-- ── SUCCESS State ── -->
//         @if (state() === 'success') {
//           <div class="state-block" id="state-success">
//             <div class="success-icon">
//               <svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
//                 <path d="M20 6L9 17l-5-5"/>
//               </svg>
//             </div>
//             <h2>تم التحقق بنجاح! 🎉</h2>
//             <p>مرحباً <strong>{{ userName() }}</strong>، سيتم تحويلك تلقائياً...</p>
//             <div class="redirect-countdown">
//               <span>الانتقال خلال</span>
//               <span class="countdown">{{ countdown() }}</span>
//               <span>ثوانٍ</span>
//             </div>
//             <button class="btn-primary" id="magic-link-go-now" (click)="navigateToDashboard()">
//               الانتقال الآن
//             </button>
//           </div>
//         }

//         <!-- ── ERROR State ── -->
//         @if (state() === 'error') {
//           <div class="state-block" id="state-error">
//             <div class="error-icon">
//               <svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
//                 <circle cx="12" cy="12" r="10"/>
//                 <line x1="12" y1="8" x2="12" y2="12"/>
//                 <line x1="12" y1="16" x2="12.01" y2="16"/>
//               </svg>
//             </div>
//             <h2>الرابط غير صالح</h2>
//             <p>{{ errorMessage() }}</p>
//             <div class="action-row">
//               <button class="btn-primary" id="magic-link-retry" (click)="retry()">
//                 حاول مرة أخرى
//               </button>
//               <a class="btn-ghost" href="/auth/login">تسجيل الدخول العادي</a>
//             </div>
//           </div>
//         }

//         <!-- ── NO TOKEN State ── -->
//         @if (state() === 'no-token') {
//           <div class="state-block" id="state-no-token">
//             <div class="info-icon">
//               <svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
//                 <circle cx="12" cy="12" r="10"/>
//                 <line x1="12" y1="16" x2="12" y2="12"/>
//                 <line x1="12" y1="8" x2="12.01" y2="8"/>
//               </svg>
//             </div>
//             <h2>لا يوجد رمز تحقق</h2>
//             <p>هذا الرابط غير مكتمل. يرجى استخدام الرابط الكامل الذي أُرسل إليك عبر التطبيق أو البريد الإلكتروني.</p>
//             <a class="btn-primary" href="/auth/login">الذهاب لتسجيل الدخول</a>
//           </div>
//         }
//       </div>
//     </div>
//   `,
//   styles: [`
//     :host { display: block; }

//     .magic-page {
//       min-height: 100vh;
//       background: #0a0a0f;
//       display: flex;
//       align-items: center;
//       justify-content: center;
//       padding: 1.5rem;
//       position: relative;
//       overflow: hidden;
//       direction: rtl;
//       font-family: 'Segoe UI', system-ui, sans-serif;
//     }

//     .orb {
//       position: absolute;
//       border-radius: 50%;
//       filter: blur(100px);
//       opacity: 0.12;
//       pointer-events: none;
//     }
//     .orb-1 {
//       width: 600px; height: 600px;
//       background: radial-gradient(circle, #6366f1, transparent);
//       top: -200px; right: -150px;
//     }
//     .orb-2 {
//       width: 500px; height: 500px;
//       background: radial-gradient(circle, #06b6d4, transparent);
//       bottom: -150px; left: -100px;
//     }

//     /* ── Card ── */
//     .magic-card {
//       position: relative;
//       width: 100%;
//       max-width: 420px;
//       background: rgba(255,255,255,0.03);
//       border: 1px solid rgba(255,255,255,0.08);
//       border-radius: 24px;
//       padding: 2.5rem;
//       backdrop-filter: blur(20px);
//       box-shadow:
//         0 0 0 1px rgba(99,102,241,0.1),
//         0 32px 64px rgba(0,0,0,0.4),
//         inset 0 1px 0 rgba(255,255,255,0.05);
//       animation: slideUp 0.5s cubic-bezier(0.16, 1, 0.3, 1);
//     }
//     @keyframes slideUp {
//       from { opacity: 0; transform: translateY(24px); }
//       to   { opacity: 1; transform: translateY(0); }
//     }

//     /* ── Brand ── */
//     .brand {
//       display: flex;
//       align-items: center;
//       gap: 0.75rem;
//       margin-bottom: 2.5rem;
//     }
//     .brand-icon {
//       width: 44px; height: 44px;
//       background: rgba(99,102,241,0.15);
//       border: 1px solid rgba(99,102,241,0.3);
//       border-radius: 12px;
//       display: flex; align-items: center; justify-content: center;
//     }
//     .brand-name {
//       font-size: 1.1rem;
//       font-weight: 700;
//       color: #fff;
//       letter-spacing: -0.02em;
//     }

//     /* ── State Block ── */
//     .state-block {
//       display: flex;
//       flex-direction: column;
//       align-items: center;
//       text-align: center;
//       gap: 1rem;
//       animation: fadeIn 0.4s ease;
//     }
//     @keyframes fadeIn {
//       from { opacity: 0; transform: scale(0.96); }
//       to   { opacity: 1; transform: scale(1); }
//     }

//     .state-block h2 {
//       font-size: 1.35rem;
//       font-weight: 700;
//       color: #fff;
//       margin: 0;
//       letter-spacing: -0.02em;
//     }
//     .state-block p {
//       color: rgba(255,255,255,0.45);
//       font-size: 0.9rem;
//       margin: 0;
//       line-height: 1.6;
//     }
//     .state-block strong { color: rgba(255,255,255,0.75); }

//     /* ── Verifying: Pulse ── */
//     .pulse-ring {
//       width: 72px; height: 72px;
//       border-radius: 50%;
//       background: rgba(99,102,241,0.1);
//       border: 1px solid rgba(99,102,241,0.25);
//       display: flex; align-items: center; justify-content: center;
//       position: relative;
//     }
//     .pulse-ring::before, .pulse-ring::after {
//       content: '';
//       position: absolute;
//       border-radius: 50%;
//       border: 1.5px solid rgba(99,102,241,0.4);
//       animation: pulse 2s ease-out infinite;
//     }
//     .pulse-ring::before { width: 100%; height: 100%; animation-delay: 0s; }
//     .pulse-ring::after  { width: 130%; height: 130%; animation-delay: 0.4s; }
//     @keyframes pulse {
//       0%   { transform: scale(1); opacity: 0.6; }
//       100% { transform: scale(1.5); opacity: 0; }
//     }
//     .pulse-dot {
//       width: 20px; height: 20px;
//       background: linear-gradient(135deg, #6366f1, #8b5cf6);
//       border-radius: 50%;
//       animation: blink 1.2s ease-in-out infinite;
//     }
//     @keyframes blink {
//       0%, 100% { opacity: 1; }
//       50% { opacity: 0.4; }
//     }

//     .progress-bar {
//       width: 100%;
//       height: 3px;
//       background: rgba(255,255,255,0.08);
//       border-radius: 2px;
//       overflow: hidden;
//       margin-top: 0.5rem;
//     }
//     .progress-fill {
//       height: 100%;
//       background: linear-gradient(90deg, #6366f1, #8b5cf6);
//       border-radius: 2px;
//       animation: progress 2s ease-in-out infinite;
//     }
//     @keyframes progress {
//       0%   { width: 0%; margin-left: 0; }
//       50%  { width: 70%; margin-left: 0; }
//       100% { width: 0%; margin-left: 100%; }
//     }

//     /* ── Success Icon ── */
//     .success-icon {
//       width: 72px; height: 72px;
//       background: rgba(34,197,94,0.12);
//       border: 1px solid rgba(34,197,94,0.3);
//       border-radius: 50%;
//       display: flex; align-items: center; justify-content: center;
//       color: #4ade80;
//       animation: success-pop 0.5s cubic-bezier(0.34, 1.56, 0.64, 1);
//     }
//     @keyframes success-pop {
//       from { transform: scale(0); opacity: 0; }
//       to   { transform: scale(1); opacity: 1; }
//     }

//     /* ── Error Icon ── */
//     .error-icon {
//       width: 72px; height: 72px;
//       background: rgba(239,68,68,0.1);
//       border: 1px solid rgba(239,68,68,0.25);
//       border-radius: 50%;
//       display: flex; align-items: center; justify-content: center;
//       color: #f87171;
//     }

//     /* ── Info Icon ── */
//     .info-icon {
//       width: 72px; height: 72px;
//       background: rgba(234,179,8,0.1);
//       border: 1px solid rgba(234,179,8,0.25);
//       border-radius: 50%;
//       display: flex; align-items: center; justify-content: center;
//       color: #fbbf24;
//     }

//     /* ── Countdown ── */
//     .redirect-countdown {
//       display: flex;
//       align-items: center;
//       gap: 0.4rem;
//       font-size: 0.85rem;
//       color: rgba(255,255,255,0.4);
//       background: rgba(255,255,255,0.04);
//       border: 1px solid rgba(255,255,255,0.08);
//       border-radius: 8px;
//       padding: 0.5rem 1rem;
//     }
//     .countdown {
//       font-size: 1.1rem;
//       font-weight: 700;
//       color: #818cf8;
//     }

//     /* ── Buttons ── */
//     .btn-primary {
//       width: 100%;
//       padding: 0.8rem;
//       background: linear-gradient(135deg, #6366f1, #8b5cf6);
//       color: #fff;
//       border: none;
//       border-radius: 12px;
//       font-size: 0.9rem;
//       font-weight: 600;
//       cursor: pointer;
//       text-decoration: none;
//       display: inline-flex;
//       align-items: center;
//       justify-content: center;
//       transition: opacity 0.2s, transform 0.15s;
//       box-shadow: 0 4px 20px rgba(99,102,241,0.3);
//     }
//     .btn-primary:hover { opacity: 0.9; transform: translateY(-1px); }

//     .btn-ghost {
//       width: 100%;
//       padding: 0.8rem;
//       background: rgba(255,255,255,0.05);
//       color: rgba(255,255,255,0.6);
//       border: 1px solid rgba(255,255,255,0.1);
//       border-radius: 12px;
//       font-size: 0.9rem;
//       font-weight: 500;
//       cursor: pointer;
//       text-decoration: none;
//       display: inline-flex;
//       align-items: center;
//       justify-content: center;
//       transition: background 0.2s, color 0.2s;
//     }
//     .btn-ghost:hover {
//       background: rgba(255,255,255,0.08);
//       color: #fff;
//     }

//     .action-row {
//       width: 100%;
//       display: flex;
//       flex-direction: column;
//       gap: 0.75rem;
//     }
//   `],
// })
// export class MagicLinkComponent implements OnInit {
//   private readonly route = inject(ActivatedRoute);
//   private readonly router = inject(Router);
//   private readonly http = inject(HttpClient);
//   private readonly auth = inject(AuthService);
//   private readonly orgContext = inject(OrgContextService);

//   // ─── State ────────────────────────────────────────────────────────────────
//   readonly state = signal<MagicLinkState>('verifying');
//   readonly errorMessage = signal('');
//   readonly userName = signal('');
//   readonly countdown = signal(3);

//   private countdownInterval: ReturnType<typeof setInterval> | null = null;

//   // ─── Lifecycle ────────────────────────────────────────────────────────────
//   ngOnInit(): void {
//     const token = this.route.snapshot.queryParamMap.get('token');

//     if (!token) {
//       this.state.set('no-token');
//       return;
//     }

//     this.verifyToken(token);
//   }

//   private verifyToken(token: string): void {
//     this.http
//       .post<MagicLinkResponse>('/api/v1/auth/magic-link/verify', { token })
//       .subscribe({
//         next: (res) => {
//           this.auth.setSession(res.token);
//           this.orgContext.setOrganizations(res.organizations ?? []);
//           this.userName.set(res.user.name);
//           this.state.set('success');
//           this.startCountdown();
//         },
//         error: (err) => {
//           const msg =
//             err?.error?.message ?? '';
//           this.errorMessage.set(msg);
//           this.state.set('error');
//         },
//       });
//   }

//   // ─── Countdown & Redirect ─────────────────────────────────────────────────
//   private startCountdown(): void {
//     this.countdownInterval = setInterval(() => {
//       const current = this.countdown();
//       if (current <= 1) {
//         this.clearCountdown();
//         this.navigateToDashboard();
//       } else {
//         this.countdown.set(current - 1);
//       }
//     }, 1000);
//   }

//   private clearCountdown(): void {
//     if (this.countdownInterval) {
//       clearInterval(this.countdownInterval);
//       this.countdownInterval = null;
//     }
//   }

//   navigateToDashboard(): void {
//     this.clearCountdown();
//     const orgId = this.orgContext.getActiveOrgId();
//     if (orgId) {
//       this.router.navigate(['/org/overview']);
//     } else {
//       this.router.navigate(['/super-admin']);
//     }
//   }

//   retry(): void {
//     window.history.back();
//   }
// }

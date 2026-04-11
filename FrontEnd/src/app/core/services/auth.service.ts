import { Injectable, signal, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthUser } from '../model/auth/authUser';
import { LoginData } from '../model/auth/loginData';
import { LoginResponse } from '../model/auth/loginResponse';
import { ResponseData } from '../model/response/responseData';
import { OrgContextService } from './org-context.service';

export type { AuthUser, LoginData };

const TOKEN_KEY = 'access_token';
const REFRESH_KEY = 'refresh_token';
const USER_KEY = 'user';

interface JwtClaims {
  sub: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress': string;
  organizationId?: string;
  organizationRole?: string;
  permission?: string[];
  role?: string | string[];
  exp: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly router = inject(Router);
  private readonly orgContext = inject(OrgContextService);
  private readonly _url = 'https://root2route.runasp.net/api/v1/auth/';

  private readonly _currentUser = signal<AuthUser | null>(this.loadUser());
  private readonly _token = signal<string | null>(this.loadToken());
  private readonly _refreshToken = signal<string | null>(localStorage.getItem(REFRESH_KEY));

  readonly currentUser = this._currentUser.asReadonly();
  readonly isAuthenticated = computed(() => !!this._token());
  readonly permissions = computed(() => this._currentUser()?.permissions ?? []);

  getToken(): string | null {
    return this._token();
  }


  login(data: LoginData): Observable<ResponseData<LoginResponse>> {
    return new Observable((observer) => {
      fetch(this._url + 'login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      })
        .then(async (res) => {
          const body = await res.json();
          if (!res.ok) {
            throw new Error(body?.message || 'Login failed');
          }
          return body as ResponseData<LoginResponse>;
        })
        .then((response) => {
          this.setSession(response.data);
          observer.next(response);
          observer.complete();
          this.navigateAfterLogin(response.data);
        })
        .catch((err: Error) => {
          observer.error(err);
        });
    });
  }

  setSession(data: LoginResponse): void {
    const claims = this.decodeToken(data.accessToken);
    const user: AuthUser = {
      id: claims.sub ?? '',
      email: claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? '',
      name: data.fullName ?? '',
      permissions: Array.isArray(claims.permission) ? claims.permission : [],
    };

    // Persist tokens & user
    localStorage.setItem(TOKEN_KEY, data.accessToken);
    localStorage.setItem(REFRESH_KEY, data.refreshToken);
    localStorage.setItem(USER_KEY, JSON.stringify(user));

    // Update signals
    this._token.set(data.accessToken);
    this._refreshToken.set(data.refreshToken);
    this._currentUser.set(user);
  }

  clearSession(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_KEY);
    localStorage.removeItem(USER_KEY);
    this._token.set(null);
    this._refreshToken.set(null);
    this._currentUser.set(null);
    this.orgContext.clearContext();
  }

  hasPermission(permission: string): boolean {
    return this.permissions().includes(permission);
  }

  // ─── Navigation After Login ───────────────────────────────────────────────
  private navigateAfterLogin(data: LoginResponse): void {
    const claims = this.decodeToken(data.accessToken);

    // 1. SuperAdmin role → go to super-admin dashboard
    const identityRoles = Array.isArray(claims.role)
      ? claims.role
      : claims.role
      ? [claims.role]
      : [];

    if (identityRoles.includes('SuperAdmin')) {
      this.router.navigate(['/super-admin']);
      return;
    }

    // 2. Check via API if user owns any organization → go to /org
    this.orgContext.myOrganization().subscribe({
      next: (orgs) => {
        if (orgs && orgs.length > 0) {
          // orgContext.setOrganizations() already picked the first org as active
          this.router.navigate(['/org']);
        } else {
          // 3. No org → unauthorized
          this.router.navigate(['/unauthorized']);
        }
      },
      error: () => {
        // If the API call fails, treat as unauthorized
        this.router.navigate(['/unauthorized']);
      },
    });
  }

  // ─── JWT Decode (no library needed) ──────────────────────────────────────
  private decodeToken(token: string): JwtClaims {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const json = decodeURIComponent(
        atob(base64)
          .split('')
          .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      return JSON.parse(json) as JwtClaims;
    } catch {
      return {} as JwtClaims;
    }
  }
  isSuperAdmin(): boolean {
    const claims = this.decodeToken(this._token()!);
    const identityRoles = Array.isArray(claims.role)
      ? claims.role
      : claims.role
      ? [claims.role]
      : [];
    return identityRoles.includes('SuperAdmin');
  }
  isLogin(): boolean{
    return this.isAuthenticated();
  }

  // ─── Persistence ──────────────────────────────────────────────────────────
  private loadToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  private loadUser(): AuthUser | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as AuthUser;
    } catch {
      return null;
    }
  }
}

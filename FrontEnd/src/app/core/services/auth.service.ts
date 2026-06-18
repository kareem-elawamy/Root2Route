import { Injectable, signal, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthUser } from '../model/auth/authUser';
import { LoginData } from '../model/auth/loginData';
import { LoginResponse } from '../model/auth/loginResponse';
import { ResponseData } from '../model/response/responseData';
import { OrgContextService } from './org-context.service';
import { UltraAlert } from '@kareem_elawamy/ultra-alert';

export type { AuthUser, LoginData };

const TOKEN_KEY = 'access_token';
const REFRESH_KEY = 'refresh_token';
const USER_KEY = 'user';

interface JwtClaims {
  sub: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress': string;
  organizationId?: string;
  organizationRole?: string;
  permission?: string | string[];
  Permission?: string | string[];
  role?: string | string[];
  Role?: string | string[]; // <--- السطر ده اللي هيحل الإيرور
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string | string[];
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
    const rawPerm = claims.permission || claims['Permission'] || [];
    const perms = Array.isArray(rawPerm) ? rawPerm : [rawPerm];

    const rawRole = claims['role'] || claims['Role'] || claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    const identityRoles = Array.isArray(rawRole) ? rawRole : rawRole ? [rawRole] : [];

    if (claims.organizationRole) {
      identityRoles.push(claims.organizationRole);
    }

    const user: AuthUser = {
      id: claims.sub ?? '',
      email: claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? '',
      name: data.fullName ?? '',
      permissions: perms,
      roles: identityRoles
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

  refreshToken(orgId?: string): Observable<ResponseData<LoginResponse>> {
    return new Observable((observer) => {
      fetch(this._url + 'refresh-token', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          accessToken: this._token(),
          refreshToken: this._refreshToken(),
          organizationId: orgId
        }),
      })
        .then(async (res) => {
          const body = await res.json();
          if (!res.ok) {
            throw new Error(body?.message || 'Refresh token failed');
          }
          return body as ResponseData<LoginResponse>;
        })
        .then((response) => {
          this.setSession(response.data);
          observer.next(response);
          observer.complete();
        })
        .catch((err: Error) => {
          observer.error(err);
        });
    });
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
    const roles = this._currentUser()?.roles ?? [];
    if (roles.includes('Owner') || roles.includes('OrganizationOwner') || roles.includes('SuperAdmin') || roles.includes('Admin')) {
      return true;
    }
    return this.permissions().includes(permission);
  }

  // ─── Navigation After Login ───────────────────────────────────────────────
  private navigateAfterLogin(data: LoginResponse): void {
    const claims = this.decodeToken(data.accessToken);

    const rawRole = claims['role'] || claims['Role'] || claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    const identityRoles = Array.isArray(rawRole)
      ? rawRole
      : rawRole
        ? [rawRole]
        : [];

    if (identityRoles.includes('SuperAdmin')) {
      this.router.navigate(['/admin-dashboard']);
      return;
    }

    this.orgContext.myOrganization().subscribe({
      next: (response: any) => {
        const orgs = response?.data || response;
        if (orgs && orgs.length > 0) {
          const firstOrgId = orgs[0].id;
          // Refresh token so the new token gets 'organizationRole' and 'permissions'
          this.refreshToken(firstOrgId).subscribe({
            next: () => {
              // After successful refresh, roles will be updated. Now we can navigate safely.
              this.router.navigate(['/company-dashboard']);
            },
            error: (err) => {
              console.error('Failed to refresh token for org:', err);
              UltraAlert.error('Refresh Token Failed: ' + err.message);
              this.router.navigate(['/unauthorized']);
            }
          });
        } else {
          // If the user has no organizations, they can't access company-dashboard
          this.router.navigate(['/user/invitations']);
        }
      },
      error: (err) => {
        console.error('Failed to load orgs during login:', err);
        UltraAlert.error('Load Orgs Failed: ' + err.message);
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
    const token = this._token();
    if (!token) return false;

    const claims = this.decodeToken(token);
    const rawRole = claims['role'] || claims['Role'] || claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    const identityRoles = Array.isArray(rawRole)
      ? rawRole
      : rawRole
        ? [rawRole]
        : [];
    return identityRoles.includes('SuperAdmin');
  }

  isLogin(): boolean {
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
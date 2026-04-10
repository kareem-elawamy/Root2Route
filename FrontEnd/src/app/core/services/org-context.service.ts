import { Injectable, signal, computed } from '@angular/core';
import { MyOrganization } from '../model/Organization/myOrganization';
import { Observable } from 'rxjs';
import { OverviewResponse } from '../model/Organization/overviewResponse';
const ACTIVE_ORG_KEY = 'r2r_active_org_id';
const ORG_LIST_KEY = 'r2r_org_list';

@Injectable({ providedIn: 'root' })
export class OrgContextService {
  private readonly _urlOrganizations = 'https://root2route.runasp.net/api/v1/organizations/';
  private readonly _urlOverview = 'https://root2route.runasp.net/api/v1/dashboard/org/';
  private readonly _activeOrgId = signal<string | null>(this.loadActiveOrgId());
  private readonly _organizations = signal<MyOrganization[]>(this.loadOrgs());
  private readonly _overview = signal<OverviewResponse | null>(null);
  readonly activeOrgId = this._activeOrgId.asReadonly();
  readonly organizations = this._organizations.asReadonly();
  readonly activeOrg = computed(() =>
    this._organizations().find((o) => o.id === this._activeOrgId())
  );
  overview(): Observable<OverviewResponse> {
    return new Observable((observer) => {
      const token = localStorage.getItem('access_token');
      fetch(this._urlOverview + this._activeOrgId() + '/overview', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
          'X-Organization-Id': this._activeOrgId()!,
        },
      })
        .then(async (res) => {
          const body = await res.json();
          if (!res.ok) {
            throw new Error(body?.message || 'Failed to load overview');
          }
          console.log(body);
          return (body.data ?? body) as OverviewResponse;
        })
        .then((overview) => {
          this._overview.set(overview);
          observer.next(overview);
          observer.complete();
        })
        .catch((err: Error) => {
          observer.error(err);
        });
    });
  }
  myOrganization(): Observable<MyOrganization[]> {
    return new Observable((observer) => {
      const token = localStorage.getItem('access_token');
      fetch(this._urlOrganizations + 'my', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
      })
        .then(async (res) => {
          const body = await res.json();
          if (!res.ok) {
            throw new Error(body?.message || 'Failed to load organizations');
          }
          console.log(body);
          return (body.data ?? body) as MyOrganization[];
        })
        .then((orgs) => {
          this.setOrganizations(orgs);
          observer.next(orgs);
          observer.complete();
        })
        .catch((err: Error) => {
          observer.error(err);
        });
    });
  }


  getActiveOrgId(): string | null {
    return this._activeOrgId();
  }

  switchOrganization(orgId: string): void {
    localStorage.setItem(ACTIVE_ORG_KEY, orgId);
    this._activeOrgId.set(orgId);
  }

  setOrganizations(orgs: MyOrganization[]): void {
    localStorage.setItem(ORG_LIST_KEY, JSON.stringify(orgs));
    this._organizations.set(orgs);

    if (!this._activeOrgId() && orgs.length > 0) {
      this.switchOrganization(orgs[0].id);
    }
  }

  clearContext(): void {
    localStorage.removeItem(ACTIVE_ORG_KEY);
    localStorage.removeItem(ORG_LIST_KEY);
    this._activeOrgId.set(null);
    this._organizations.set([]);
  }

  private loadActiveOrgId(): string | null {
    return localStorage.getItem(ACTIVE_ORG_KEY);
  }

  private loadOrgs(): MyOrganization[] {
    const raw = localStorage.getItem(ORG_LIST_KEY);
    return raw ? (JSON.parse(raw) as MyOrganization[]) : [];
  }
}

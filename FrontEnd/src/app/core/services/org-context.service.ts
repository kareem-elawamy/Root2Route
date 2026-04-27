import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MyOrganization } from '../model/Organization/myOrganization';
import { Observable, throwError } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { OverviewResponse } from '../model/Organization/overviewResponse';
import { environment } from '../../../environments/environment';
import { ActivityChartResponse } from '../model/Organization/activity-chart-response';

const ACTIVE_ORG_KEY = 'r2r_active_org_id';
const ORG_LIST_KEY = 'r2r_org_list';

@Injectable({ providedIn: 'root' })
export class OrgContextService {
  private readonly http = inject(HttpClient);
  private readonly _urlOrganizations = environment.apiUrl + 'organizations/';
  private readonly _urlOverview = environment.apiUrl + 'dashboard/org/';
  private readonly _activeOrgId = signal<string | null>(this.loadActiveOrgId());
  private readonly _organizations = signal<MyOrganization[]>([]);
  private readonly _overview = signal<OverviewResponse | null>(null);
  private readonly _activeChar = signal<ActivityChartResponse[]>([]);
  readonly activeOrgId = this._activeOrgId.asReadonly();
  readonly organizations = this._organizations.asReadonly();
  readonly activeChar = this._activeChar.asReadonly();
  readonly overviewData = this._overview.asReadonly();
  readonly activeOrg = computed(() =>
    this._organizations().find((o) => o.id === this._activeOrgId())
  );

  overview(): Observable<OverviewResponse> {
    const orgId = this._activeOrgId();
    if (!orgId) {
      return throwError(() => new Error('Active organization ID is not set.'));
    }

    return this.http.get<any>(this._urlOverview + orgId + '/overview').pipe(
      map(body => (body.data ?? body) as OverviewResponse),
      tap(overview => this._overview.set(overview))
    );
  }
  charData(orgId: string, months: number): Observable<ActivityChartResponse[]> {
    if (!orgId) {
      return throwError(() => new Error('Active organization ID is not set.'));
    }
    return this.http.get<any>(this._urlOverview + orgId + '/activity-chart?months=' + months).pipe(
      map(body => (body.data ?? body) as ActivityChartResponse[]),
      tap(chartData => this._activeChar.set(chartData))
    );
  }
  myOrganization(): Observable<MyOrganization[]> {
    return this.http.get<any>(this._urlOrganizations + 'my').pipe(
      map(body => (body.data ?? body) as MyOrganization[]),
      tap(orgs => this.setOrganizations(orgs))
    );
  }

  getActiveOrgId(): string | null {
    return this._activeOrgId();
  }

  switchOrganization(orgId: string): void {
    localStorage.setItem(ACTIVE_ORG_KEY, orgId);
    this._activeOrgId.set(orgId);
  }

  setOrganizations(orgs: MyOrganization[]): void {
    const orgIds = orgs.map(o => o.id);
    localStorage.setItem(ORG_LIST_KEY, JSON.stringify(orgIds));
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
}

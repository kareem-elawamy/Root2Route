import { Injectable, signal, computed } from '@angular/core';
import { MyOrganization } from '../model/Organization/myOrganization';
import { Observable } from 'rxjs';
import { OverviewResponse } from '../model/Organization/overviewResponse';
import { LatestOrder } from '../model/Organization/latestOrder';
import { LiveBid } from '../model/Organization/liveBid';
import { ActivityChartItem } from '../model/Organization/activityChartItem';
import { ProductOverviewResponse } from '../model/Organization/productOverviewResponse';
import { ProductResponse } from '../model/Organization/productResponse';

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

  activityChart(months: number = 6): Observable<ActivityChartItem[]> {
    return new Observable((observer) => {
      const token = localStorage.getItem('access_token');
      fetch(`${this._urlOverview}${this._activeOrgId()}/activity-chart?months=${months}`, {
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
            throw new Error(body?.message || 'Failed to load activity chart');
          }
          return (body.data ?? body) as ActivityChartItem[];
        })
        .then((data) => {
          observer.next(data);
          observer.complete();
        })
        .catch((err: Error) => {
          observer.error(err);
        });
    });
  }

  liveBids(limit: number = 20): Observable<LiveBid[]> {
    return new Observable((observer) => {
      const token = localStorage.getItem('access_token');
      fetch(`${this._urlOverview}${this._activeOrgId()}/live-bids?limit=${limit}`, {
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
            throw new Error(body?.message || 'Failed to load live bids');
          }
          return (body.data ?? body) as LiveBid[];
        })
        .then((data) => {
          observer.next(data);
          observer.complete();
        })
        .catch((err: Error) => {
          observer.error(err);
        });
    });
  }

  latestOrders(limit: number = 10): Observable<LatestOrder[]> {
    return new Observable((observer) => {
      const token = localStorage.getItem('access_token');
      fetch(`${this._urlOverview}${this._activeOrgId()}/latest-orders?limit=${limit}`, {
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
            throw new Error(body?.message || 'Failed to load latest orders');
          }
          return (body.data ?? body) as LatestOrder[];
        })
        .then((data) => {
          observer.next(data);
          observer.complete();
        })
        .catch((err: Error) => {
          observer.error(err);
        });
    });
  }

  productOverview(): Observable<ProductOverviewResponse> {
    return new Observable((observer) => {
      const token = localStorage.getItem('access_token');
      fetch(`${this._urlOverview}${this._activeOrgId()}/product-overview`, {
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
            throw new Error(body?.message || 'Failed to load product overview');
          }
          return (body.data ?? body) as ProductOverviewResponse;
        })
        .then((data) => {
          observer.next(data);
          observer.complete();
        })
        .catch((err: Error) => {
          observer.error(err);
        });
    });
  }

  products(pageNumber: number = 1, pageSize: number = 10): Observable<ProductResponse[]> {
    return new Observable((observer) => {
      const token = localStorage.getItem('access_token');
      fetch(`${this._urlOverview}${this._activeOrgId()}/products?pageNumber=${pageNumber}&pageSize=${pageSize}`, {
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
            throw new Error(body?.message || 'Failed to load products');
          }
          return (body.data ?? body) as ProductResponse[];
        })
        .then((data) => {
          observer.next(data);
          observer.complete();
        })
        .catch((err: Error) => {
          observer.error(err);
        });
    });
  }

  addProduct(formData: FormData): Observable<any> {
    return new Observable((observer) => {
      const token = localStorage.getItem('access_token');
      fetch('https://root2route.runasp.net/api/v1/product/Add', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
        body: formData,
      })
        .then(async (res) => {
          const body = await res.json();
          if (!res.ok) {
            throw new Error(body?.message || 'Failed to add product');
          }
          return body;
        })
        .then((data) => {
          observer.next(data);
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

import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);
  
  private baseUrl = 'https://root2route.runasp.net/api/v1/dashboard/org';

  getOverview(orgId: string, days?: number): Observable<any> {
    let params = new HttpParams();
    if (days) {
      params = params.set('days', days.toString());
    }
    return this.http.get(`${this.baseUrl}/${orgId}/overview`, { params });
  }

  getActivityChart(orgId: string, months: number = 6): Observable<any> {
    const params = new HttpParams().set('months', months.toString());
    return this.http.get(`${this.baseUrl}/${orgId}/activity-chart`, { params });
  }

  getLiveBids(orgId: string, limit: number = 20): Observable<any> {
    const params = new HttpParams().set('limit', limit.toString());
    return this.http.get(`${this.baseUrl}/${orgId}/live-bids`, { params });
  }

  getLatestOrders(orgId: string, limit: number = 10): Observable<any> {
    const params = new HttpParams().set('limit', limit.toString());
    return this.http.get(`${this.baseUrl}/${orgId}/latest-orders`, { params });
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);

  // اللينك الأساسي للداشبورد (إحصائيات)
  private baseUrl = 'https://root2route.runasp.net/api/v1/dashboard/superadmin';

  // اللينك الأساسي للمنظمات
  private orgsUrl = 'https://root2route.runasp.net/api/v1/organizations';

  // 1. فانكشن الإحصائيات (موجودة من الأول)
  getOverviewStats(): Observable<any> {
    return this.http.get(`${this.baseUrl}/overview-stats`);
  }

  // 2. الفانكشن الجديدة: بتجيب المنظمات اللي في الانتظار
  getPendingOrganizations(): Observable<any> {
    return this.http.get(`${this.baseUrl}/organizations/pending`);
  }
}
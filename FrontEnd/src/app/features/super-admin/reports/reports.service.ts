import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReportsService {
  // استخدمنا inject زي الداش بورد عشان الكود يبقى أحدث وأنظف
  private http = inject(HttpClient);

  // 🟢 ضفنا اللينك الأساسي للباك إند (Live Server)
  private baseUrl = 'https://root2route.runasp.net/api/v1/dashboard/superadmin';

  getFinancials(): Observable<any> {
    return this.http.get(`${this.baseUrl}/financials`);
  }

  getOverviewStats(): Observable<any> {
    return this.http.get(`${this.baseUrl}/overview-stats`);
  }
}
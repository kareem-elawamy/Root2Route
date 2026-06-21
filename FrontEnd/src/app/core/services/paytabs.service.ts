import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PaytabsService {
  private http = inject(HttpClient);
  private baseUrl = 'https://root2route.runasp.net/api/v1/payment/paytabs';
  private analyticsUrl = 'https://root2route.runasp.net/api/v1/payment/paytabs-analytics';

  createPayment(orderId: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/create/${orderId}`, {});
  }

  verifyPayment(tranRef: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/verify/${tranRef}`);
  }

  getOrganizationAnalytics(from: string, to: string): Observable<any> {
    const params = new HttpParams().set('from', from).set('to', to);
    return this.http.get(`${this.analyticsUrl}/organization`, { params });
  }

  getSuperAdminAnalytics(from: string, to: string): Observable<any> {
    const params = new HttpParams().set('from', from).set('to', to);
    return this.http.get(`${this.analyticsUrl}/super-admin`, { params });
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private http = inject(HttpClient);
  private baseUrl = 'https://root2route.runasp.net/api/v1/admin/payments';

  getAllPayments(page: number = 1, size: number = 20, from?: string, to?: string, status?: string): Observable<any> {
    let params = new HttpParams()
      .set('PageNumber', page.toString())
      .set('PageSize', size.toString());

    if (from) params = params.set('From', from);
    if (to) params = params.set('To', to);
    if (status) params = params.set('Status', status);

    return this.http.get(this.baseUrl, { params });
  }
}

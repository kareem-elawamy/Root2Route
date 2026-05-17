import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private http = inject(HttpClient);
  
  private baseUrl = 'https://root2route.runasp.net/api/v1/Order';

  getReceivedOrders(organizationId: string, pageNumber: number = 1, pageSize: number = 10): Observable<any> {
    const params = new HttpParams()
      .set('PageNumber', pageNumber.toString())
      .set('PageSize', pageSize.toString());

    return this.http.get(`${this.baseUrl}/Received/${organizationId}`, { params });
  }

  changeStatus(command: { orderId: string; status: string; currentUserId: string; note?: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.put(`${this.baseUrl}/ChangeStatus`, command, { headers });
  }

  cancelOrder(orderId: string): Observable<any> {
    return this.http.put(`${this.baseUrl}/CancelOrder/${orderId}`, {});
  }
}

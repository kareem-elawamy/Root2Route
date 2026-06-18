import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BuyerOrderService {
  private http = inject(HttpClient);
  private baseUrl = 'https://root2route.runasp.net/api/v1/Order';

  createOrder(payload: {
    productId: string;
    quantity: number;
    shippingAddressId: string;
    notes?: string;
  }): Observable<any> {
    return this.http.post(`${this.baseUrl}/Create`, payload);
  }

  getMyOrders(pageNumber: number = 1, pageSize: number = 10): Observable<any> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get(`${this.baseUrl}/MyOrders`, { params });
  }

  getOrderById(id: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/${id}`);
  }

  cancelOrder(id: string): Observable<any> {
    return this.http.put(`${this.baseUrl}/CancelOrder/${id}`, {});
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ShipmentsService {
  private http = inject(HttpClient);
  
  private baseUrl = 'https://root2route.runasp.net/api/v1/shipments';

  getMyAddresses(): Observable<any> {
    return this.http.get(`${this.baseUrl}/addresses`);
  }

  addAddress(command: { street: string; city: string; state: string; country: string; zipCode: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/addresses`, command);
  }

  dispatchOrder(command: { orderId: string; carrierName: string; trackingNumber: string; driverPhone?: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/dispatch`, command);
  }

  updateShipmentStatus(id: string, newStatus: number): Observable<any> {
    return this.http.put(`${this.baseUrl}/status/${id}`, { newStatus });
  }
}

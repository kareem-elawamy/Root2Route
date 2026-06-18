import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuctionService {
  private http = inject(HttpClient);
  
  private baseUrl = 'https://root2route.runasp.net/api/v1/auctions';

  // غيرنا الكلمة هنا بناءً على الـ Swagger
  getActiveAuctions(): Observable<any> {
    return this.http.get(`${this.baseUrl}/GetActive`);
  }

  getOrganizationAuctions(organizationId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/my-organization/${organizationId}`);
  }

  createAuction(payload: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/create`, payload);
  }

  cancelAuction(auctionId: string): Observable<any> {
    return this.http.put(`${this.baseUrl}/${auctionId}/cancel`, {});
  }

  getCompletedAuctions(): Observable<any> {
    return this.http.get(`${this.baseUrl}/GetCompleted`);
  }

  getAllAuctions(): Observable<any> {
    return this.http.get(`${this.baseUrl}/GetAll`);
  }

  getAuctionById(id: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/${id}`);
  }

  getAuctionBids(auctionId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/${auctionId}/bids`);
  }

  updateAuction(auctionId: string, payload: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/${auctionId}/update`, payload);
  }

  placeBid(auctionId: string, amount: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/${auctionId}/bid`, { amount });
  }

  checkoutWonAuction(auctionId: string, data: { shippingAddressId: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/${auctionId}/checkout`, data);
  }

  getMyWonAuctions(): Observable<any> {
    return this.http.get(`${this.baseUrl}/my-won`);
  }

  getMyParticipatedAuctions(): Observable<any> {
    return this.http.get(`${this.baseUrl}/my-participated`);
  }
}
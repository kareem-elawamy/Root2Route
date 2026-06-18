import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BuyerAuctionService {
  private http = inject(HttpClient);
  private baseUrl = 'https://root2route.runasp.net/api/v1/auctions';

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

  getActiveAuctions(): Observable<any> {
    return this.http.get(`${this.baseUrl}/GetActive`);
  }

  getAuctionById(id: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/${id}`);
  }

  getAuctionBids(auctionId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/${auctionId}/bids`);
  }
}

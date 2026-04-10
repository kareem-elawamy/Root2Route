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
}
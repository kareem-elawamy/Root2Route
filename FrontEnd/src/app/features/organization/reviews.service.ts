import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReviewsService {
  private http = inject(HttpClient);
  
  private baseUrl = 'https://root2route.runasp.net/api/v1/reviews';

  getOrganizationReviews(orgId: string, pageNumber: number = 1, pageSize: number = 10): Observable<any> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get(`${this.baseUrl}/organization/${orgId}`, { params });
  }

  addReview(payload: { organizationId: string; orderId: string; rating: number; comment?: string }): Observable<any> {
    return this.http.post(this.baseUrl, payload);
  }
}

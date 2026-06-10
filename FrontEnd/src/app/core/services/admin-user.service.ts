import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminUserService {
  private http = inject(HttpClient);
  private baseUrl = 'https://root2route.runasp.net/api/v1/admin/users';

  getAllUsers(page: number = 1, size: number = 20, search?: string, suspended?: boolean): Observable<any> {
    let params = new HttpParams()
      .set('PageNumber', page.toString())
      .set('PageSize', size.toString());

    if (search) params = params.set('Search', search);
    if (suspended !== undefined && suspended !== null) params = params.set('IsSuspended', suspended.toString());

    return this.http.get(this.baseUrl, { params });
  }

  suspendUser(userId: string, suspend: boolean): Observable<any> {
    return this.http.put(`${this.baseUrl}/${userId}/suspend`, { userId, suspend });
  }
}

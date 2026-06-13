import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminUserService {
  private http = inject(HttpClient);
  private baseUrl = 'https://root2route.runasp.net/api/v1/admin/users';

  getAllUsers(page: number = 1, size: number = 20, search?: string, blocked?: boolean): Observable<any> {
    let params = new HttpParams()
      .set('PageNumber', page.toString())
      .set('PageSize', size.toString());

    if (search) params = params.set('Search', search);
    if (blocked !== undefined && blocked !== null) params = params.set('IsBlocked', blocked.toString());

    return this.http.get(this.baseUrl, { params });
  }

  blockUser(userId: string, block: boolean): Observable<any> {
    return this.http.put(`${this.baseUrl}/${userId}/block`, { userId, block });
  }
}

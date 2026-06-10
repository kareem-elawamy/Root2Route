import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuditLogService {
  private http = inject(HttpClient);
  private baseUrl = 'https://root2route.runasp.net/api/v1/admin/audit-logs';

  getAuditLogs(page: number = 1, size: number = 50, entity?: string, action?: string, from?: string, to?: string): Observable<any> {
    let params = new HttpParams()
      .set('PageNumber', page.toString())
      .set('PageSize', size.toString());

    if (entity) params = params.set('EntityName', entity);
    if (action) params = params.set('Action', action);
    if (from) params = params.set('From', from);
    if (to) params = params.set('To', to);

    return this.http.get(this.baseUrl, { params });
  }
}

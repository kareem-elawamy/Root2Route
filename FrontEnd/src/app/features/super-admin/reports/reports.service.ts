import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReportsService {
  constructor(private http: HttpClient) {}

  getFinancials(): Observable<any> {
    return this.http.get('/api/v1/dashboard/superadmin/financials');
  }

  getOverviewStats(): Observable<any> {
    return this.http.get('/api/v1/dashboard/superadmin/overview-stats');
  }
}

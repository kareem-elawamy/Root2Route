import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WithdrawalService {
  private http = inject(HttpClient);
  private baseUrl = 'https://root2route.runasp.net/api/v1/withdrawals';

  requestWithdrawal(payload: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/request`, payload);
  }

  getOrganizationWithdrawals(): Observable<any> {
    return this.http.get(`${this.baseUrl}/organization`);
  }

  getPendingWithdrawals(): Observable<any> {
    return this.http.get(`${this.baseUrl}/pending`);
  }

  approveWithdrawal(withdrawalId: string, adminNote?: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/approve`, { withdrawalId, adminNote });
  }

  rejectWithdrawal(withdrawalId: string, adminNote?: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/reject`, { withdrawalId, adminNote });
  }

  processWithdrawal(withdrawalId: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/process`, { withdrawalId });
  }
}

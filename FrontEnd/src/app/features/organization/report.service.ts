import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private http = inject(HttpClient);

  private baseUrl = 'https://root2route.runasp.net/api/v1/dashboard/org';

  // ── Orders ──
  exportOrdersCsv(orgId: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${orgId}/export/orders/csv`, { responseType: 'blob' });
  }

  exportOrdersPdf(orgId: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${orgId}/export/orders/pdf`, { responseType: 'blob' });
  }

  // ── Products ──
  exportProductsCsv(orgId: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${orgId}/export/products/csv`, { responseType: 'blob' });
  }

  exportProductsPdf(orgId: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${orgId}/export/products/pdf`, { responseType: 'blob' });
  }

  // ── Financial ──
  exportFinancialCsv(orgId: string, months: number = 6): Observable<Blob> {
    const params = new HttpParams().set('months', months.toString());
    return this.http.get(`${this.baseUrl}/${orgId}/export/financial/csv`, { params, responseType: 'blob' });
  }

  exportFinancialPdf(orgId: string, months: number = 6): Observable<Blob> {
    const params = new HttpParams().set('months', months.toString());
    return this.http.get(`${this.baseUrl}/${orgId}/export/financial/pdf`, { params, responseType: 'blob' });
  }

  // ── Download Helper ──
  triggerDownload(blob: Blob, filename: string): void {
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrganizationService {
  private http = inject(HttpClient);

  // اللينك الأساسي للمنظمات
  private baseUrl = 'https://root2route.runasp.net/api/v1/organizations';
  private dashboardUrl = 'https://root2route.runasp.net/api/v1/dashboard/superadmin';

  // فانكشن بتجيب كل المنظمات
  getAllOrganizations(): Observable<any> {
    return this.http.get(`${this.baseUrl}`);
  }

  // تحديث حالة المنظمة (موافقة، رفض، إلخ)
  updateStatus(id: string, newStatus: number): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.put(`${this.baseUrl}/${id}/status`, newStatus, { headers });
  }

  approveOrganization(id: string): Observable<any> {
    return this.http.put(`${this.dashboardUrl}/organizations/${id}/approve`, null);
  }

  rejectOrganization(id: string, reason: string): Observable<any> {
    return this.http.put(`${this.dashboardUrl}/organizations/${id}/reject?reason=${encodeURIComponent(reason)}`, null);
  }

  // Create a new organization
  createOrganization(data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}`, data);
  }

  // Update an existing organization
  updateOrganization(data: FormData): Observable<any> {
    return this.http.put(`${this.baseUrl}`, data);
  }
}
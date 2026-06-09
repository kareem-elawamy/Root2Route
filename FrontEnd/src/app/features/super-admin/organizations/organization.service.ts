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

  // فانكشن بتجيب كل المنظمات
  getAllOrganizations(): Observable<any> {
    return this.http.get(`${this.baseUrl}`);
  }

  // تحديث حالة المنظمة (موافقة، رفض، إلخ)
  updateStatus(id: string, newStatus: number): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.put(`${this.baseUrl}/${id}/status`, newStatus, { headers });
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
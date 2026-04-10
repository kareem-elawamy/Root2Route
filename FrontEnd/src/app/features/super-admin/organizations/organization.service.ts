import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
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
}
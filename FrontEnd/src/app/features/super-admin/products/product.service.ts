import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private http = inject(HttpClient);
  
  private baseUrl = 'https://root2route.runasp.net/api/v1/product';

  getAllProducts(pageNumber: number = 1, pageSize: number = 10, search?: string): Observable<any> {
    let params = new HttpParams()
      .set('PageNumber', pageNumber.toString())
      .set('PageSize', pageSize.toString());
      
    if (search) {
      params = params.set('Search', search);
    }

    return this.http.get(`${this.baseUrl}/GetAll`, { params });
  }

  getProductsByOrg(orgId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/Organization/${orgId}`);
  }

  getPendingProductsByOrg(orgId: string): Observable<any> {
    return this.http.get(`https://root2route.runasp.net/api/v1/dashboard/org/${orgId}/products/pending`);
  }

  createProduct(formData: FormData): Observable<any> {
    return this.http.post(`${this.baseUrl}/Add`, formData);
  }

  changeStatus(productId: string, newStatus: number): Observable<any> {
    const body = {
      productId: productId,
      status: newStatus
    };
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.put(`${this.baseUrl}/ChangeStatus`, body, { headers });
  }

  deleteProduct(productId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Delete/${productId}`);
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AiLabService {
  private http = inject(HttpClient);
  
  // اللينك الأساسي من الصورة بتاعتك
  private baseUrl = 'https://root2route.runasp.net/api/v1/dashboard/superadmin';

  getAiDashboardData(): Observable<any> {
    // forkJoin بتبعت الطلبات دي كلها للسيرفر مع بعض في نفس اللحظة
    return forkJoin({
      topDiseases: this.http.get(`${this.baseUrl}/ml/top-diseases`),
      accuracyTrend: this.http.get(`${this.baseUrl}/ml/accuracy-trend`),
      heatmap: this.http.get(`${this.baseUrl}/ml/disease-heatmap`) // لو هتحتاجها مستقبلاً
    });
  }
}
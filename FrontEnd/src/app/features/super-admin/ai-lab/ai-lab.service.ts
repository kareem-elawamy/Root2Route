import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AiLabService {
  private http = inject(HttpClient);
  
  private baseUrl = 'https://root2route.runasp.net/api/v1/dashboard/superadmin';
  private analysisUrl = 'https://root2route.runasp.net/api/v1/model-analysis';

  getAiDashboardData(): Observable<any> {
    return forkJoin({
      topDiseases: this.http.get(`${this.baseUrl}/ml/top-diseases`),
      accuracyTrend: this.http.get(`${this.baseUrl}/ml/accuracy-trend`),
      heatmap: this.http.get(`${this.baseUrl}/ml/disease-heatmap`)
    });
  }

  analyzeImage(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('Image', file);
    return this.http.post(`${this.analysisUrl}/analyze`, formData);
  }
}
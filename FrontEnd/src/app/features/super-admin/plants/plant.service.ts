import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PlantService {
  private http = inject(HttpClient);
  
  private baseUrl = 'https://root2route.runasp.net/api/v1/plant-info';

  getAllPlants(): Observable<any> {
    return this.http.get(`${this.baseUrl}/all`);
  }

  createPlant(formData: FormData): Observable<any> {
    return this.http.post(`${this.baseUrl}/create`, formData);
  }

  editPlant(formData: FormData): Observable<any> {
    return this.http.put(`${this.baseUrl}/edit`, formData);
  }

  // DELETE with id in URL path (matches [FromRoute] Guid id on backend)
  deletePlant(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/delete/${id}`);
  }

  // --- Plant Guide Steps ---
  private stepBaseUrl = 'https://root2route.runasp.net/api/v1/plant-guide-step';

  getStepsByPlantId(plantId: string): Observable<any> {
    return this.http.get(`${this.stepBaseUrl}/plant-id/${plantId}`);
  }

  createStep(step: any): Observable<any> {
    return this.http.post(`${this.stepBaseUrl}/create`, step);
  }

  editStep(step: any): Observable<any> {
    return this.http.put(`${this.stepBaseUrl}/edit`, step);
  }

  deleteStep(id: string): Observable<any> {
    return this.http.delete(`${this.stepBaseUrl}/delete/${id}`);
  }
}

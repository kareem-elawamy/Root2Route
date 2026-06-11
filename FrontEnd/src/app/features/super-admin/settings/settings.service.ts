import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  private http = inject(HttpClient);
  private dashboardUrl = 'https://root2route.runasp.net/api/v1/dashboard/superadmin';

  updatePlatformFee(newFee: number): Observable<any> {
    return this.http.put(`${this.dashboardUrl}/settings/platform-fee?newFee=${newFee}`, null);
  }
}

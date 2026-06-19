import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FcmService {
  private http = inject(HttpClient);
  private baseUrl = 'https://root2route.runasp.net/api/v1/users/fcm-token';

  saveFcmToken(token: string): Observable<any> {
    return this.http.post(this.baseUrl, { token });
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private http = inject(HttpClient);
  private baseUrl = 'https://root2route.runasp.net/api/v1/notifications';

  getNotifications(): Observable<any> {
    return this.http.get(`${this.baseUrl}`);
  }

  getUnreadCount(): Observable<any> {
    return this.http.get(`${this.baseUrl}/unread-count`);
  }

  markAsRead(id: string): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}/read`, {});
  }

  markAllAsRead(): Observable<any> {
    return this.http.put(`${this.baseUrl}/read-all`, {});
  }
}

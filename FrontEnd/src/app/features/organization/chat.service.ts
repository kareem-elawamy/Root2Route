import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private http = inject(HttpClient);
  
  private baseUrl = 'https://root2route.runasp.net/api/v1/chat';

  getMyRooms(pageNumber: number = 1, pageSize: number = 20): Observable<any> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get(`${this.baseUrl}/my-rooms`, { params });
  }

  getHistory(chatRoomId: string, pageNumber: number = 1, pageSize: number = 50): Observable<any> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get(`${this.baseUrl}/${chatRoomId}/history`, { params });
  }

  sendMessage(formData: FormData): Observable<any> {
    return this.http.post(`${this.baseUrl}/send`, formData);
  }

  acceptOffer(command: { offerId: string; currentUserId?: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/accept-offer`, { offerMessageId: command.offerId });
  }

  rejectOffer(command: { offerId: string; currentUserId?: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/reject-offer`, { offerMessageId: command.offerId });
  }

  markAsRead(roomId: string): Observable<any> {
    return this.http.put(`${this.baseUrl}/${roomId}/read`, {});
  }

  startChat(command: { otherUserId: string; productId?: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/start`, command);
  }

  closeChat(roomId: string): Observable<any> {
    return this.http.put(`${this.baseUrl}/${roomId}/close`, {});
  }

  deleteMessage(messageId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/messages/${messageId}`);
  }

  getRoomDetails(roomId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/${roomId}/details`);
  }
}

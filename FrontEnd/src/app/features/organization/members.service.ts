import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http = inject(HttpClient);
  
  private membersUrl = 'https://root2route.runasp.net/api/v1/organization-members';
  private invitationsUrl = 'https://root2route.runasp.net/api/v1/organization-invitations';

  getMembers(organizationId: string): Observable<any> {
    return this.http.get(`${this.membersUrl}/by-organization/${organizationId}`);
  }

  removeMember(memberId: string): Observable<any> {
    return this.http.put(`${this.membersUrl}/${memberId}/remove`, {});
  }

  getInvitations(organizationId: string): Observable<any> {
    const params = new HttpParams().set('organizationId', organizationId);
    return this.http.get(this.invitationsUrl, { params });
  }

  sendInvitation(command: { email: string; organizationId: string; roleId?: string }): Observable<any> {
    return this.http.post(`${this.invitationsUrl}/send`, command);
  }

  revokeInvitation(invitationId: string): Observable<any> {
    return this.http.post(`${this.invitationsUrl}/revoke`, { InvitationId: invitationId });
  }
}

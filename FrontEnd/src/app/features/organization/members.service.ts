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
  private rolesUrl = 'https://root2route.runasp.net/api/v1/organization-roles';

  getOrganizationRoles(organizationId: string): Observable<any> {
    return this.http.get(`${this.rolesUrl}/by-organization/${organizationId}`);
  }

  getSystemPermissions(): Observable<any> {
    return this.http.get(`${this.rolesUrl}/system-permissions`);
  }

  createOrganizationRole(command: any): Observable<any> {
    return this.http.post(`${this.rolesUrl}/create`, command);
  }

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

  changeOrganizationOwner(orgId: string, newOwnerId: string): Observable<any> {
    const headers = { 'Content-Type': 'application/json' };
    return this.http.put(`https://root2route.runasp.net/api/v1/organizations/${orgId}/change-owner`, `"${newOwnerId}"`, { headers });
  }

  getMyInvitations(): Observable<any> {
    return this.http.get(`${this.invitationsUrl}/my`);
  }

  acceptInvitation(invitationId: string, token: string): Observable<any> {
    const body = { invitationId };
    return this.http.post(`${this.invitationsUrl}/accept?token=${encodeURIComponent(token)}`, body);
  }

  deleteRole(roleId: string): Observable<any> {
    return this.http.delete(`${this.rolesUrl}/${roleId}`);
  }

  updateRole(roleId: string, data: any): Observable<any> {
    return this.http.put(`${this.rolesUrl}/${roleId}`, data);
  }

  assignRoleToMember(memberId: string, roleId: string): Observable<any> {
    return this.http.put(`${this.membersUrl}/${memberId}/role`, { roleId });
  }
}


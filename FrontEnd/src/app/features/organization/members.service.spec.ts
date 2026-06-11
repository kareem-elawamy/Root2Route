import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { MembersService } from './members.service';

describe('MembersService', () => {
  let service: MembersService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        MembersService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(MembersService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getMembers should fetch members for an organization', () => {
    const mockMembers = [{ id: '1', userId: 'user-1' }];
    const orgId = 'org-123';

    service.getMembers(orgId).subscribe(members => {
      expect(members).toEqual(mockMembers);
    });

    const req = httpMock.expectOne(`https://root2route.runasp.net/api/v1/organization-members/by-organization/${orgId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockMembers);
  });

  it('getOrganizationRoles should fetch roles', () => {
    const mockRoles = [{ id: 'role-1', name: 'Admin' }];
    const orgId = 'org-123';

    service.getOrganizationRoles(orgId).subscribe(roles => {
      expect(roles).toEqual(mockRoles);
    });

    const req = httpMock.expectOne(`https://root2route.runasp.net/api/v1/organization-roles/by-organization/${orgId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockRoles);
  });

  it('sendInvitation should POST invitation data', () => {
    const command = { email: 'test@example.com', organizationId: 'org-123' };
    const mockResponse = { succeeded: true };

    service.sendInvitation(command).subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`https://root2route.runasp.net/api/v1/organization-invitations/send`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(command);
    req.flush(mockResponse);
  });

  it('assignRoleToMember should PUT role data', () => {
    const memberId = 'member-123';
    const roleId = 'role-1';
    const mockResponse = { succeeded: true };

    service.assignRoleToMember(memberId, roleId).subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`https://root2route.runasp.net/api/v1/organization-members/${memberId}/role`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({ roleId });
    req.flush(mockResponse);
  });
});

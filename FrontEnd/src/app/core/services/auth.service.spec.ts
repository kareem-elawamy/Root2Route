import { TestBed } from '@angular/core/testing';
import { AuthService } from './auth.service';
import { OrgContextService } from './org-context.service';
import { Router } from '@angular/router';

describe('AuthService', () => {
  let service: AuthService;
  let routerSpy: { navigate: any };
  let orgContextSpy: { clearContext: any };

  beforeEach(() => {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    routerSpy = { navigate: vi.fn() };
    orgContextSpy = { clearContext: vi.fn() };

    // Clear local storage to avoid test pollution
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        { provide: Router, useValue: routerSpy },
        { provide: OrgContextService, useValue: orgContextSpy }
      ]
    });
    service = TestBed.inject(AuthService);
  });

  afterEach(() => {
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('isAuthenticated should return false initially if no token', () => {
    expect(service.isAuthenticated()).toBe(false);
  });

  it('hasPermission should return true if user is Owner/Admin', () => {
    const fakeJwtPayload = {
      sub: '123',
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress': 'test@test.com',
      role: 'Owner'
    };
    
    const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
    const payload = btoa(JSON.stringify(fakeJwtPayload));
    const token = `${header}.${payload}.signature`;

    service.setSession({
      accessToken: token,
      refreshToken: 'fake-refresh',
      fullName: 'Test User',
      expireAt: new Date().toISOString(),
      role: ['Owner'],
      message: 'OK',
      firstOrganizationId: ''
    });

    expect(service.hasPermission('ManageProducts')).toBe(true);
  });

  it('hasPermission should return true if user has specific permission', () => {
    const fakeJwtPayload = {
      sub: '123',
      role: 'User',
      permission: 'ManageProducts'
    };
    
    const token = `header.${btoa(JSON.stringify(fakeJwtPayload))}.signature`;

    service.setSession({
      accessToken: token,
      refreshToken: 'fake-refresh',
      fullName: 'Test User',
      expireAt: new Date().toISOString(),
      role: ['User'],
      message: 'OK',
      firstOrganizationId: ''
    });

    expect(service.hasPermission('ManageProducts')).toBe(true);
    expect(service.hasPermission('DeleteProducts')).toBe(false);
  });

  it('clearSession should remove tokens and clear context', () => {
    // Setup initial state
    localStorage.setItem('access_token', 'token');
    
    service.clearSession();

    expect(localStorage.getItem('access_token')).toBeNull();
    expect(service.getToken()).toBeNull();
    expect(orgContextSpy.clearContext).toHaveBeenCalled();
  });

  it('isSuperAdmin should return true if role is SuperAdmin', () => {
    const fakeJwtPayload = {
      sub: '123',
      role: 'SuperAdmin'
    };
    
    const token = `header.${btoa(JSON.stringify(fakeJwtPayload))}.signature`;

    service.setSession({
      accessToken: token,
      refreshToken: 'fake-refresh',
      fullName: 'Test User',
      expireAt: new Date().toISOString(),
      role: ['SuperAdmin'],
      message: 'OK',
      firstOrganizationId: ''
    });

    expect(service.isSuperAdmin()).toBe(true);
  });
});

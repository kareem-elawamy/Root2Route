import { TestBed } from '@angular/core/testing';
import { OrgContextService } from './org-context.service';

describe('OrgContextService', () => {
  let service: OrgContextService;

  beforeEach(() => {
    // Clear local storage to ensure clean state
    localStorage.clear();
    
    TestBed.configureTestingModule({
      providers: [OrgContextService]
    });
    service = TestBed.inject(OrgContextService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('switchOrganization should update activeOrgId and localStorage', () => {
    expect(service.activeOrgId()).toBeNull();
    
    service.switchOrganization('org-123');
    
    expect(service.activeOrgId()).toBe('org-123');
    expect(localStorage.getItem('r2r_active_org_id')).toBe('org-123');
  });

  it('setOrganizations should update organizations signal and switch to first if none active', () => {
    const orgs = [{ id: 'org-1', name: 'Org 1' }];
    
    service.setOrganizations(orgs);
    
    expect(service.organizations()).toEqual(orgs);
    expect(service.activeOrgId()).toBe('org-1');
  });

  it('clearContext should reset state to null/empty', () => {
    service.switchOrganization('org-123');
    service.setOrganizations([{ id: 'org-123', name: 'Org' }]);
    
    expect(service.activeOrgId()).toBe('org-123');
    expect(service.organizations().length).toBe(1);
    
    service.clearContext();
    
    expect(service.activeOrgId()).toBeNull();
    expect(service.organizations().length).toBe(0);
    expect(localStorage.getItem('r2r_active_org_id')).toBeNull();
    expect(localStorage.getItem('r2r_org_list')).toBeNull();
  });
});

import { TestBed } from '@angular/core/testing';

import { Organization } from './organization';

describe('Organization', () => {
  let service: Organization;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Organization);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

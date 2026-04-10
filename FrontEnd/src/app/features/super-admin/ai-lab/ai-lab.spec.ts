import { TestBed } from '@angular/core/testing';

import { AiLab } from './ai-lab';

describe('AiLab', () => {
  let service: AiLab;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AiLab);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

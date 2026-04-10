import { TestBed } from '@angular/core/testing';

import { Auction } from './auction';

describe('Auction', () => {
  let service: Auction;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Auction);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

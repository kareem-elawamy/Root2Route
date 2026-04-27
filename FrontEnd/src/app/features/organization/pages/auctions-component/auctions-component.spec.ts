import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuctionsComponent } from './auctions-component';

describe('AuctionsComponent', () => {
  let component: AuctionsComponent;
  let fixture: ComponentFixture<AuctionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AuctionsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AuctionsComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

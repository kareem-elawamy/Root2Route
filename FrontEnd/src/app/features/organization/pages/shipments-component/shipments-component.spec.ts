import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShipmentsComponent } from './shipments-component';

describe('ShipmentsComponent', () => {
  let component: ShipmentsComponent;
  let fixture: ComponentFixture<ShipmentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShipmentsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShipmentsComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

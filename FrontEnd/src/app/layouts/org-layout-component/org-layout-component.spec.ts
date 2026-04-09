import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrgLayoutComponent } from './org-layout-component';

describe('OrgLayoutComponent', () => {
  let component: OrgLayoutComponent;
  let fixture: ComponentFixture<OrgLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrgLayoutComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrgLayoutComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

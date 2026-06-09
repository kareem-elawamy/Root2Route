import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OverviewComponent } from './overview-component';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { AuthService } from '../../../../core/services/auth.service';
import { DashboardService } from '../../dashboard.service';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { signal } from '@angular/core';

describe('OverviewComponent', () => {
  let component: OverviewComponent;
  let fixture: ComponentFixture<OverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OverviewComponent],
      providers: [
        { 
          provide: OrgContextService, 
          useValue: { activeOrg: signal({ id: '1', name: 'Test Org' }) } 
        },
        { 
          provide: AuthService, 
          useValue: { currentUser: signal({ name: 'Test User' }), isLogin: () => true } 
        },
        { 
          provide: DashboardService, 
          useValue: { 
            getOverview: () => of({}), 
            getLatestOrders: () => of([]), 
            getLiveBids: () => of([]) 
          } 
        },
        {
          provide: ActivatedRoute,
          useValue: { params: of({}) }
        }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OverviewComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

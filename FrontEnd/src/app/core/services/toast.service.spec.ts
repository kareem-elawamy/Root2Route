import { TestBed } from '@angular/core/testing';
import { ToastService } from './toast.service';
import { UltraAlert } from '@kareem_elawamy/ultra-alert';

vi.mock('@kareem_elawamy/ultra-alert', () => ({
  UltraAlert: {
    toast: vi.fn().mockResolvedValue({})
  }
}));

describe('ToastService', () => {
  let service: ToastService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ToastService]
    });
    service = TestBed.inject(ToastService);
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('success should call UltraAlert.toast with success type', () => {
    service.success('Operation successful');
    expect(UltraAlert.toast).toHaveBeenCalledWith('Operation successful', { type: 'success', autoClose: 4000 });
  });

  it('error should call UltraAlert.toast with error type', () => {
    service.error('Operation failed');
    expect(UltraAlert.toast).toHaveBeenCalledWith('Operation failed', { type: 'error', autoClose: 6000 });
  });

  it('warning should call UltraAlert.toast with warning type', () => {
    service.warning('Take caution');
    expect(UltraAlert.toast).toHaveBeenCalledWith('Take caution', { type: 'warning', autoClose: 5000 });
  });

  it('info should call UltraAlert.toast with info type', () => {
    service.info('Just so you know');
    expect(UltraAlert.toast).toHaveBeenCalledWith('Just so you know', { type: 'info', autoClose: 4000 });
  });
});

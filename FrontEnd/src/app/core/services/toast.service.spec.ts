import { TestBed } from '@angular/core/testing';
import { ToastService } from './toast.service';

describe('ToastService', () => {
  let service: ToastService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ToastService]
    });
    service = TestBed.inject(ToastService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('success should add a success toast', () => {
    service.success('Operation successful');
    const toasts = service.toasts();
    
    expect(toasts.length).toBe(1);
    expect(toasts[0].type).toBe('success');
    expect(toasts[0].message).toBe('Operation successful');
  });

  it('error should add an error toast', () => {
    service.error('Operation failed');
    const toasts = service.toasts();
    
    expect(toasts.length).toBe(1);
    expect(toasts[0].type).toBe('error');
    expect(toasts[0].message).toBe('Operation failed');
  });

  it('warning should add a warning toast', () => {
    service.warning('Take caution');
    const toasts = service.toasts();
    
    expect(toasts.length).toBe(1);
    expect(toasts[0].type).toBe('warning');
    expect(toasts[0].message).toBe('Take caution');
  });

  it('info should add an info toast', () => {
    service.info('Just so you know');
    const toasts = service.toasts();
    
    expect(toasts.length).toBe(1);
    expect(toasts[0].type).toBe('info');
    expect(toasts[0].message).toBe('Just so you know');
  });

  it('should not exceed 5 toasts', () => {
    for (let i = 0; i < 7; i++) {
      service.info(`Message ${i}`);
    }
    
    const toasts = service.toasts();
    expect(toasts.length).toBe(5);
    // Should keep the last 5
    expect(toasts[0].message).toBe('Message 2');
    expect(toasts[4].message).toBe('Message 6');
  });

  it('should remove toast after duration', () => {
    vi.useFakeTimers();
    service.success('Test msg', 4000);
    
    expect(service.toasts().length).toBe(1);
    
    vi.advanceTimersByTime(2000);
    expect(service.toasts().length).toBe(1); // Still there
    
    vi.advanceTimersByTime(2000); // 4000 total
    expect(service.toasts().length).toBe(0); // Removed
    
    vi.useRealTimers();
  });

  it('removeToast should remove a toast manually', () => {
    service.info('Message 1');
    service.info('Message 2');
    
    const toasts = service.toasts();
    expect(toasts.length).toBe(2);
    
    service.removeToast(toasts[0].id);
    
    const updatedToasts = service.toasts();
    expect(updatedToasts.length).toBe(1);
    expect(updatedToasts[0].message).toBe('Message 2');
  });
});

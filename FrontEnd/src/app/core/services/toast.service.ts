import { Injectable, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface ToastItem {
  id: number;
  type: ToastType;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private idCounter = 0;
  toasts = signal<ToastItem[]>([]);

  success(message: string, duration = 4000) {
    this.addToast('success', message, duration);
  }

  error(message: string, duration = 6000) {
    this.addToast('error', message, duration);
  }

  warning(message: string, duration = 5000) {
    this.addToast('warning', message, duration);
  }

  info(message: string, duration = 4000) {
    this.addToast('info', message, duration);
  }

  private addToast(type: ToastType, message: string, duration: number) {
    const id = ++this.idCounter;
    
    this.toasts.update(current => {
      // Keep max 5 toasts on screen
      const newToasts = [...current, { id, type, message }];
      if (newToasts.length > 5) {
        return newToasts.slice(newToasts.length - 5);
      }
      return newToasts;
    });

    setTimeout(() => {
      this.removeToast(id);
    }, duration);
  }

  removeToast(id: number) {
    this.toasts.update(current => current.filter(t => t.id !== id));
  }
}

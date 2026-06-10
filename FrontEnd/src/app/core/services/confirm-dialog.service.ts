import { Injectable, signal } from '@angular/core';
import { Subject, Observable } from 'rxjs';

export interface ConfirmDialogOptions {
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  isDestructive?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ConfirmDialogService {
  options = signal<ConfirmDialogOptions | null>(null);
  private resultSubject = new Subject<boolean>();

  open(options: ConfirmDialogOptions): Observable<boolean> {
    this.options.set({
      confirmLabel: 'Confirm',
      cancelLabel: 'Cancel',
      isDestructive: false,
      ...options
    });
    this.resultSubject = new Subject<boolean>();
    return this.resultSubject.asObservable();
  }

  confirm() {
    this.resultSubject.next(true);
    this.resultSubject.complete();
    this.options.set(null);
  }

  cancel() {
    this.resultSubject.next(false);
    this.resultSubject.complete();
    this.options.set(null);
  }
}

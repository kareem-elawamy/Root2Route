import { Injectable } from '@angular/core';
import { UltraAlert } from '@kareem_elawamy/ultra-alert';

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
  async open(options: ConfirmDialogOptions): Promise<boolean> {
    if (options.isDestructive) {
      const result = await UltraAlert.deleteConfirm(options.title, options.message);
      return result.isConfirmed;
    }
    const result = await UltraAlert.confirm(options.title, options.message);
    return result.isConfirmed;
  }
}

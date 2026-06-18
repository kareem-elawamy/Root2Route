import { Injectable } from '@angular/core';
import { UltraAlert } from '@kareem_elawamy/ultra-alert';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  success(message: string) {
    UltraAlert.toast(message, { type: 'success', autoClose: 4000 });
  }

  error(message: string) {
    UltraAlert.toast(message, { type: 'error', autoClose: 6000 });
  }

  warning(message: string) {
    UltraAlert.toast(message, { type: 'warning', autoClose: 5000 });
  }

  info(message: string) {
    UltraAlert.toast(message, { type: 'info', autoClose: 4000 });
  }
}

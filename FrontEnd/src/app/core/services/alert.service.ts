import { Injectable } from '@angular/core';
import { UltraAlert } from '@kareem_elawamy/ultra-alert';
@Injectable({
    providedIn: 'root'
})
export class AlertService {
    showSuccess(message: string) {
        UltraAlert.success(message);
    }
    showError(message: string) {
        UltraAlert.error(message);
    }
    showWarning(message: string) {
        UltraAlert.warning(message);
    }
    showInfo(message: string) {
        UltraAlert.info(message);
    }
    showLoading(message: string) {
        UltraAlert.loading(message);
    }

}
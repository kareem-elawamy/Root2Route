import 'zone.js'; // السطر السحري اللي هيحل المشكلة
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { UltraAlert } from '@kareem_elawamy/ultra-alert';
import '@kareem_elawamy/ultra-alert/css';

UltraAlert.configure({
  theme: 'light',
  toast: {
    position: 'top-end',
    duration: 4000,
    maxVisible: 5,
  },
  defaults: {
    confirmButton: { text: 'Confirm' },
    cancelButton:  { text: 'Cancel'  },
  },
});

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));
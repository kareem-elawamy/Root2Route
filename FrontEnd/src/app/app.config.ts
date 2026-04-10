  feature/add-seader
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import {
  provideRouter,
  withComponentInputBinding,
  withRouterConfig,
} from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
 
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { authInterceptor } from './core/interceptors/auth.interceptor';
  main

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { orgContextInterceptor } from './core/interceptors/org-context.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
  feature/add-seader
    provideBrowserGlobalErrorListeners(),
    provideRouter(
      routes,
      withComponentInputBinding(),
      withRouterConfig({ paramsInheritanceStrategy: 'always' })
    ),
    provideHttpClient(
      withInterceptors([
        authInterceptor,
        orgContextInterceptor,
      ])
    ),
    provideAnimationsAsync(),
  ],
};
 
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor]))
  ]
};
  main

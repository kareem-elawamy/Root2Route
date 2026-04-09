import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

/**
 * Super Admin Layout Shell.
 * Wraps all super-admin routes with the shared sidebar, topbar, etc.
 * Sub-components (sidebar, navbar) will be added here as the feature grows.
 */
@Component({
  selector: 'app-super-admin-layout',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <div class="super-admin-layout">
      <!-- TODO: <app-super-admin-sidebar /> -->
      <!-- TODO: <app-super-admin-topbar /> -->
      <main class="layout-content">
        <router-outlet />
      </main>
    </div>
  `,
  styles: [`
    .super-admin-layout {
      display: flex;
      min-height: 100vh;
    }
    .layout-content {
      flex: 1;
      overflow: auto;
    }
  `],
})
export class SuperAdminLayoutComponent {}

import { Component, inject } from '@angular/core';
import { Router, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-super-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './main-layout.html' // 🟢 السطر ده اللي هيجيب السايد بار بتاعك
})
export class SuperAdminLayoutComponent {
  private router = inject(Router);

  logout() {
    // 1. امسح التوكن من المتصفح
    localStorage.removeItem('token');

    // 2. ارمي راوتر الأنجولار خالص واستخدم راوتر المتصفح عشان نمسح الذاكرة (Memory)
    window.location.href = '/auth/login';
  }
}
import { Component, inject } from '@angular/core';
import { Router, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

// 🟢 اتأكد إن السطر ده موجود عشان نستخدم خدمة المصادقة
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-super-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './main-layout.html'
})
export class SuperAdminLayoutComponent {
  private router = inject(Router);
  private authService = inject(AuthService); // 🟢 بنحقن الخدمة هنا

  logout() {
    // 1. بننادي على الفانكشن اللي التيم عاملها بتمسح كل حاجة من الذاكرة
    this.authService.clearSession();

    // 2. بنخرج لصفحة اللوجين
    this.router.navigate(['/auth/login']);
  }
}
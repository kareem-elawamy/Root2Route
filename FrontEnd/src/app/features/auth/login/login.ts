import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html'
})
export class Login {
  loginData = { userName: '', password: '' };

  private authService = inject(AuthService);
  private router = inject(Router);
  onLogin() {
    console.log(`sinding`, this.loginData);

    this.authService.login(this.loginData).subscribe({
      next: (response: any) => {
        console.log(`done sinding`, response);


        const token = response.data.accessToken;
        if (token) {
          localStorage.setItem('token', token);
        }

        this.router.navigate(['/dashboard']);
      },
      error: (err: any) => {
        console.error('فشل تسجيل الدخول:', err);
        alert('تأكد من الإيميل والباسوورد!');
      }
    });
  }
}
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { LoginData } from '../../../../core/model/auth/loginData';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class LoginComponent {
  private readonly loginService = inject(AuthService);
  private readonly fb = inject(FormBuilder);

  readonly isLoading = signal(false);
  readonly errorMessage = signal('');
  readonly showPassword = signal(false);

  readonly loginForm = this.fb.group({
    userName: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    isRememberMe: [false],
  });

  isFieldInvalid(field: string): boolean {
    const control = this.loginForm.get(field);
    return !!(control?.invalid && control?.touched);
  }

  login(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    const data: LoginData = {
      userName: this.loginForm.value.userName ?? '',
      password: this.loginForm.value.password ?? '',
      isRememberMe: this.loginForm.value.isRememberMe ?? false,
    };

    this.loginService.login(data).subscribe({
      next: () => {
        this.isLoading.set(false);
      },
      error: (err: Error) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.message || 'Login failed. Please try again.');
      },
    });
  }
}

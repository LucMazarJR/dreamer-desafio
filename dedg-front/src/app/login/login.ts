import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import {
  LucideBuilding2,
  LucideMail,
  LucideLock,
  LucideEye,
  LucideEyeOff,
  LucideArrowRight,
} from '@lucide/angular';
import { Footer } from '../shared/ui/footer/footer';
import { AuthService } from '../core/auth/auth.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule, LucideBuilding2, LucideMail, LucideLock, LucideEye, LucideEyeOff, LucideArrowRight, Footer],
  templateUrl: './login.html',
})
export class Login {
  email = '';
  password = '';
  rememberMe = false;
  showPassword = signal(false);
  loading = signal(false);
  errorMessage = signal('');

  constructor(private auth: AuthService, private router: Router) {}

  togglePassword() {
    this.showPassword.update((v) => !v);
  }

  onSubmit() {
    if (!this.email || !this.password) return;
    this.loading.set(true);
    this.errorMessage.set('');

    this.auth.login(this.email, this.password).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: () => {
        this.loading.set(false);
        this.errorMessage.set('E-mail ou senha inválidos.');
      },
    });
  }
}

import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  LucideBuilding2,
  LucideMail,
  LucideLock,
  LucideEye,
  LucideEyeOff,
  LucideArrowRight,
} from '@lucide/angular';
import { Footer } from "../shared/ui/footer/footer";

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

  togglePassword() {
    this.showPassword.update((v) => !v);
  }

  onSubmit() {
    // execução teste para o botão
    console.log({ email: this.email, rememberMe: this.rememberMe });
  }
}

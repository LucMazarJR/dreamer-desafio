import { Component, inject } from '@angular/core';
import { LucideLogOut, LucideMenu } from '@lucide/angular';
import { LayoutService } from '../../layout.service';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-header',
  imports: [LucideLogOut, LucideMenu],
  templateUrl: './header.html',
})
export class Header {
  layout = inject(LayoutService);
  auth = inject(AuthService);
}

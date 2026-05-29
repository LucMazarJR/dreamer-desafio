import { Component, inject, computed } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import {
  LucideBuilding2,
  LucideLayoutDashboard,
  LucideHistory,
  LucideUsers,
  LucideCalendar,
  LucideX,
} from '@lucide/angular';
import { LayoutService } from '../../layout.service';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-side-bar',
  imports: [RouterLink, RouterLinkActive, LucideBuilding2, LucideLayoutDashboard, LucideHistory, LucideUsers, LucideCalendar, LucideX],
  templateUrl: './side-bar.html',
})
export class SideBar {
  layout = inject(LayoutService);
  auth = inject(AuthService);

  userName = computed(() => this.auth.currentUser()?.name ?? '');
  userInitials = computed(() =>
    this.userName()
      .split(' ')
      .slice(0, 2)
      .map((w) => w[0] ?? '')
      .join('')
      .toUpperCase()
  );
  userRoleLabel = computed(() => {
    const labels: Record<string, string> = {
      Collaborator: 'Colaborador',
      Manager: 'Gestor',
      HrAdmin: 'Administrador RH',
    };
    return labels[this.auth.currentUser()?.role ?? ''] ?? '';
  });
}

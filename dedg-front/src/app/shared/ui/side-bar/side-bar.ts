import { Component, inject } from '@angular/core';
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

@Component({
  selector: 'app-side-bar',
  imports: [RouterLink, RouterLinkActive, LucideBuilding2, LucideLayoutDashboard, LucideHistory, LucideUsers, LucideCalendar, LucideX],
  templateUrl: './side-bar.html',
})
export class SideBar {
  layout = inject(LayoutService);

  // TODO: substituir por dados do usuário autenticado
  user = {
    name: 'Ricardo Silva',
    role: 'Desenvolvedor Sênior',
    initials: 'RS',
  };
}

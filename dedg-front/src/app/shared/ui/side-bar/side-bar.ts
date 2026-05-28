import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import {
  LucideBuilding2,
  LucideLayoutDashboard,
  LucideHistory,
  LucideUsers,
  LucideCalendar,
} from '@lucide/angular';

@Component({
  selector: 'app-side-bar',
  imports: [RouterLink, RouterLinkActive, LucideBuilding2, LucideLayoutDashboard, LucideHistory, LucideUsers, LucideCalendar],
  templateUrl: './side-bar.html',
})
export class SideBar {
  // TODO: substituir por dados do usuário autenticado
  user = {
    name: 'Ricardo Silva',
    role: 'Desenvolvedor Sênior',
    initials: 'RS',
  };
}

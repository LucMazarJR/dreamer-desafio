import { Component, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { RouterLink } from '@angular/router';
import {
  LucideLogIn,
  LucideCoffee,
  LucideUtensils,
  LucideLogOut,
  LucideClipboardList,
  LucideCircleCheckBig,
} from '@lucide/angular';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, LucideLogIn, LucideCoffee, LucideUtensils, LucideLogOut, LucideClipboardList, LucideCircleCheckBig],
  templateUrl: './dashboard.html',
})
export class Dashboard implements OnInit, OnDestroy {
  private timer: ReturnType<typeof setInterval> | null = null;
  private now = signal(new Date());

  time = computed(() =>
    this.now().toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false })
  );

  date = computed(() =>
    this.now().toLocaleDateString('pt-BR', { day: 'numeric', month: 'long', year: 'numeric' })
  );

  ngOnInit() {
    this.timer = setInterval(() => this.now.set(new Date()), 1000);
  }

  ngOnDestroy() {
    if (this.timer) clearInterval(this.timer);
  }

  // TODO: substituir por dados da API
  lastEntry = {
    type: 'Entrada Realizada',
    time: '08:02',
    label: 'Hoje, 24 de Outubro',
  };

  // TODO: substituir por dados da API
  daySummary = {
    hoursWorked: '04:12',
  };
}

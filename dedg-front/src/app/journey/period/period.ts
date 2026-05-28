import { Component, signal, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import {
  LucideCirclePlus,
  LucideRefreshCcw,
  LucideLock,
} from '@lucide/angular';
import { AuthService } from '../../core/auth/auth.service';
import { MonthlyPeriodService, MonthlyPeriod } from '../../core/monthly-period/monthly-period.service';

@Component({
  selector: 'app-period',
  imports: [RouterLink, LucideCirclePlus, LucideRefreshCcw, LucideLock],
  templateUrl: './period.html',
})
export class Period implements OnInit {
  auth = inject(AuthService);
  private periodSvc = inject(MonthlyPeriodService);

  periods = signal<MonthlyPeriod[]>([]);
  loading = signal(true);
  loadError = signal('');
  actionError = signal('');
  acting = signal(false);

  showNewForm = signal(false);
  newYear = signal(new Date().getFullYear());
  newMonth = signal(new Date().getMonth() + 1);

  readonly years = Array.from({ length: 3 }, (_, i) => new Date().getFullYear() - 1 + i);
  readonly months = Array.from({ length: 12 }, (_, i) => i + 1);

  ngOnInit() {
    this.loadPeriods();
  }

  private loadPeriods() {
    this.loading.set(true);
    this.loadError.set('');
    this.periodSvc.getAll().subscribe({
      next: (list) => {
        this.periods.set(list);
        this.loading.set(false);
      },
      error: () => {
        this.loadError.set('Não foi possível carregar os períodos. Tente recarregar a página.');
        this.loading.set(false);
      },
    });
  }

  toggleNewForm() {
    this.showNewForm.update((v) => !v);
    this.actionError.set('');
  }

  openPeriod() {
    if (this.acting()) return;
    this.acting.set(true);
    this.actionError.set('');
    this.periodSvc.create(this.newYear(), this.newMonth()).subscribe({
      next: (period) => {
        // Insert at the correct position (descending order)
        this.periods.update((list) =>
          [period, ...list].sort((a, b) => b.year !== a.year ? b.year - a.year : b.month - a.month)
        );
        this.showNewForm.set(false);
        this.acting.set(false);
      },
      error: (err) => {
        this.acting.set(false);
        this.actionError.set(err?.error?.message ?? 'Erro ao criar período. Tente novamente.');
      },
    });
  }

  startReview(period: MonthlyPeriod) {
    if (this.acting()) return;
    this.acting.set(true);
    this.actionError.set('');
    this.periodSvc.startReview(period.id).subscribe({
      next: (updated) => {
        this.periods.update((list) => list.map((p) => (p.id === updated.id ? updated : p)));
        this.acting.set(false);
      },
      error: (err) => {
        this.acting.set(false);
        this.actionError.set(err?.error?.message ?? 'Erro ao iniciar revisão. Tente novamente.');
      },
    });
  }

  closePeriod(period: MonthlyPeriod) {
    if (this.acting()) return;
    const userId = this.auth.currentUser()?.userId;
    if (!userId) return;
    this.acting.set(true);
    this.actionError.set('');
    this.periodSvc.close(period.id, userId).subscribe({
      next: (updated) => {
        this.periods.update((list) => list.map((p) => (p.id === updated.id ? updated : p)));
        this.acting.set(false);
      },
      error: (err) => {
        this.acting.set(false);
        this.actionError.set(err?.error?.message ?? 'Erro ao fechar período. Tente novamente.');
      },
    });
  }

  monthName(month: number): string {
    const name = new Date(2000, month - 1).toLocaleDateString('pt-BR', { month: 'long' });
    return name.charAt(0).toUpperCase() + name.slice(1);
  }

  startDate(p: MonthlyPeriod): string {
    return `01/${String(p.month).padStart(2, '0')}/${p.year}`;
  }

  endDate(p: MonthlyPeriod): string {
    const last = new Date(p.year, p.month, 0).getDate();
    return `${last}/${String(p.month).padStart(2, '0')}/${p.year}`;
  }

  formatClosedAt(iso: string): string {
    const normalized = /Z$|[+-]\d{2}:?\d{2}$/.test(iso) ? iso : iso + 'Z';
    return new Date(normalized).toLocaleDateString('pt-BR', {
      day: '2-digit', month: '2-digit', year: 'numeric',
    });
  }
}

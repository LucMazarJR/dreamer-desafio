import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import {
  LucideCirclePlus,
  LucideRefreshCcw,
  LucideLock,
} from '@lucide/angular';
import { AuthService } from '../../core/auth/auth.service';

type PeriodStatus = 'Open' | 'InReview' | 'Closed';

interface MonthlyPeriod {
  id: number;
  year: number;
  month: number;
  status: PeriodStatus;
  closedAt?: string; // ISO: "YYYY-MM-DD", presente somente quando Closed
}

@Component({
  selector: 'app-period',
  imports: [RouterLink, LucideCirclePlus, LucideRefreshCcw, LucideLock],
  templateUrl: './period.html',
})
export class Period {
  auth = inject(AuthService);

  // TODO: substituir por dados da API — GET /api/monthlyperiods
  periods: MonthlyPeriod[] = [
    { id: 1, year: 2026, month: 5, status: 'Open' },
    { id: 2, year: 2026, month: 4, status: 'InReview' },
    { id: 3, year: 2026, month: 3, status: 'Closed', closedAt: '2026-04-03' },
    { id: 4, year: 2026, month: 2, status: 'Closed', closedAt: '2026-03-04' },
  ];

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
    const [y, m, d] = iso.split('-');
    return `${d}/${m}/${y}`;
  }
}
